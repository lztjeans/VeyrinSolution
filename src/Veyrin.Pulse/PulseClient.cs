using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Veyrin.Core.Exceptions;
using Veyrin.Core.Logging;
using Veyrin.Core.Serialization;
using Veyrin.Core.Validation;
using Veyrin.Extend.Policy;

namespace Veyrin.Pulse;

/// <summary>
/// 內部預設值提供者，確保在非 DI 環境下也能獲得穩定的基礎設施
/// </summary>
internal static class VeyrinPulseDefaults
{
    // 使用 Lazy 確保 HttpClient 在全域只有一個實例，避免 Socket 耗盡
    public static readonly Lazy<HttpClient> SharedClient = new(() => new HttpClient());

    // 提供一個基本的 Console Logger，若連 Console 都不需要則可改用 NullLogger
    //public static ILogger<PulseClient> CreateDefaultLogger()
    //{
    //    using var factory = LoggerFactory.Create(builder =>
    //    {
    //        //builder.AddConsole();
    //        //builder.ad
    //        builder.SetMinimumLevel(LogLevel.Information);
    //    });
    //    return factory.CreateLogger<PulseClient>();
    //}
    public static ILogger<PulseClient> CreateDefaultLogger()
    {
        return AppLogger.For<PulseClient>(); ;
    }
}

//public interface IPulseClient { }

public sealed class PulseClient//: IPulseClient
{
    private readonly HttpClient _client;
    private readonly ILogger<PulseClient> _logger;
    private readonly IRetryPolicy _retryPolicy;

    /// <summary>
    /// 彈性建構子：
    /// 1. 在 DI 環境中，容器會自動注入所有參數。
    /// 2. 在非 DI 環境中，不傳參數即可快速啟動。
    /// </summary>
    public PulseClient(HttpClient? client = null, ILogger<PulseClient>? logger = null, IRetryPolicy? retryPolicy = null)
    {
        // 若未提供 client，使用 Veyrin 全域共享的實例
        _client = client ?? VeyrinPulseDefaults.SharedClient.Value;

        // 若未提供 logger，建立一個預設的 Console Logger
        _logger = logger ?? VeyrinPulseDefaults.CreateDefaultLogger();

        // 若未提供策略，預設使用 FixedRetryPolicy
        _retryPolicy = retryPolicy ?? new FixedRetryPolicy();
    }
    /// <summary>
    /// 靜態工廠：提供更語義化的快速建立方式
    /// </summary>
    public static PulseClient Create(string? baseUrl = null)
    {
        var pulse = new PulseClient();
        if (StringUtils.IsNotEmpty(baseUrl))
            pulse._client.BaseAddress = new Uri(baseUrl);
        return pulse;
    }

    #region Header Control

    public PulseClient SetHeader(string name, string value)
    {
        _client.DefaultRequestHeaders.Remove(name);
        _client.DefaultRequestHeaders.Add(name, value);
        return this;
    }

    public void ConfigureDefaultHeaders(Action<HttpRequestHeaders> configure)
    {
        configure(_client.DefaultRequestHeaders);
    }

    public PulseClient SetBearerToken(string token) => SetHeader("Authorization", $"Bearer {token}");

    public void ClearHeaders() => _client.DefaultRequestHeaders.Clear();

    #endregion

    #region Public HTTP Methods

    public Task<T?> GetAsync<T>(string uri, CancellationToken ct = default)
            => SendInternalAsync<T, object>(HttpMethod.Get, uri, null, ct);

    public Task<T?> PostAsync<T, V>(string uri, V payload, CancellationToken ct = default)
        => SendInternalAsync<T, V>(HttpMethod.Post, uri, payload, ct);

    public Task<T?> PutAsync<T, V>(string uri, V payload, CancellationToken ct = default)
        => SendInternalAsync<T, V>(HttpMethod.Put, uri, payload, ct);

    public Task<T?> PatchAsync<T, V>(string uri, V payload, CancellationToken ct = default) =>
        SendInternalAsync<T, V>(HttpMethod.Patch, uri, payload, ct);

    public Task<T?> DeleteAsync<T>(string uri, CancellationToken ct = default)
            => SendInternalAsync<T, object>(HttpMethod.Delete, uri, null, ct);

    public Task<HttpResponseMessage> SendRawAsync(HttpMethod method, string uri, HttpContent? content = null, CancellationToken cancellationToken = default)
    {
        return _retryPolicy.ExecuteAsync(async () =>
        {
            using var request = new HttpRequestMessage(method, uri) { Content = content };
            var response = await _client.SendAsync(request, cancellationToken);
            return response.EnsureSuccessStatusCode();
        }, _logger, $"Raw-{method}");
    }

    #endregion

    #region File & Stream

    public Task<Stream> DownloadStreamAsync(string uri, CancellationToken cancellationToken = default) =>
        _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }, _logger, "DownloadStream");

    //public async Task DownloadFileAsync(string uri, string filePath, CancellationToken cancellationToken = default)
    //{
    //    using var stream = await DownloadStreamAsync(uri, cancellationToken);
    //    using var file = File.Create(filePath);
    //    await stream.CopyToAsync(file, cancellationToken);
    //}
    public async Task DownloadFileAsync(string uri, string filePath, CancellationToken ct = default)
    {
        Guard.NotEmpty(filePath);

        var stream = await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(ct);
        }, _logger, "DownloadFile");

        using (stream)
        using (var file = File.Create(filePath))
        {
            await stream.CopyToAsync(file, ct);
        }
    }

    /// <summary>
    /// 上傳檔案或 multipart/form-data POST
    /// </summary>
    public Task<T?> UploadFileAsync<T>(string uri, MultipartFormDataContent content, CancellationToken cancellationToken = default)
    {
        return _retryPolicy.ExecuteAsync(async () =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
            using var response = await _client.SendAsync(request, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);

            if (!response.IsSuccessStatusCode)
                HandleHttpError(response.StatusCode, uri);

            if (string.IsNullOrWhiteSpace(responseContent) || responseContent.Trim() == "null")
                return default;

            // 非 JSON 回傳的彈性處理
            if (typeof(T) == typeof(string))
                return (T)(object)responseContent;

            if (typeof(T) == typeof(byte[]))
                return (T)(object)await response.Content.ReadAsByteArrayAsync(cancellationToken);

            return JsonSerializerHelper.DeserializeJson<T>(responseContent);
        }, _logger, "UploadFile");
    }
    #endregion

    #region Core Logic

    private Task<T?> SendInternalAsync<T, V>(HttpMethod method, string uri, V? payload, CancellationToken ct)
    {
        return _retryPolicy.ExecuteAsync(async () =>
        {
            using var request = new HttpRequestMessage(method, uri);

            if (payload != null && method != HttpMethod.Get && method != HttpMethod.Delete)
                request.Content = JsonContent.Create(payload);

            using var response = await _client.SendAsync(request, ct);

            // 記錄狀態資訊
            _logger.LogInformation("[Pulse] {Method} {Uri} -> {StatusCode}", method, uri, response.StatusCode);

            if (!response.IsSuccessStatusCode)
                HandleHttpError(response.StatusCode, uri);

            // 處理空回傳
            if (response.StatusCode == HttpStatusCode.NoContent) return default;

            // --- 優化點：針對二進位資料直接處理，不轉 string ---
            if (typeof(T) == typeof(byte[]))
            {
                var bytes = await response.Content.ReadAsByteArrayAsync(ct);
                return (T)(object)bytes;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            return ConvertResponse<T>(content, response);

        }, _logger, method.Method);
    }

    #endregion



    #region Helpers

    private T? ConvertResponse<T>(string content, HttpResponseMessage response)
    {
        var type = typeof(T);

        // 1. 如果目標是 String，直接回傳原始內容
        if (type == typeof(string))
            return (T)(object)content;

        // 2. 如果目標是 Byte Array (例如下載小檔案或圖片)
        if (type == typeof(byte[]))
        {
            // 這裡我們異步讀取的邏輯通常在 SendInternalAsync 處理，
            // 但若到此處，可嘗試轉換 content (Base64) 或報錯
            throw new ValidationException("Byte[] 轉換應在流處理階段完成，不建議透過字串轉換。", nameof(T));
        }

        // 3. 根據 Content-Type 進行防禦性檢查
        var contentType = response.Content.Headers.ContentType?.MediaType;
        if (contentType != null && !contentType.Contains("json") && !type.IsPrimitive)
        {
            _logger.LogWarning("[Pulse] 回傳的 Content-Type 為 {Type}，但嘗試解析為 {Target}。可能會失敗。", contentType, type.Name);
        }

        // 4. 使用反序列化
        try
        {

            return JsonSerializerHelper.DeserializeJson<T>(content);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[Pulse] JSON 解析失敗。原始內容: {Content}", content);
            throw;
        }
    }

    private static void HandleHttpError(HttpStatusCode statusCode, string uri)
    {
        int code = (int)statusCode;
        string msg = code switch
        {
            >= 400 and < 500 => $"Client-side error ({code}) encountered while calling {uri}.",
            >= 500 => $"Server-side error ({code}) encountered at {uri}.",
            _ => $"Unexpected status code ({code}) from {uri}."
        };

        // 拋出請求異常，讓 Retry Policy 捕捉
        throw new HttpRequestException(msg, null, statusCode);
    }


    #endregion
}