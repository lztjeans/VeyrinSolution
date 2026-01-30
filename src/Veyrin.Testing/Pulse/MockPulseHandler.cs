using System.Net;

namespace Veyrin.Testing.Pulse;

public class MockPulseHandler : DelegatingHandler
{
    private readonly Dictionary<string, HttpResponseMessage> _responses = new();

    public void Setup(string url, HttpStatusCode code, string jsonResponse)
    {
        _responses[url] = new HttpResponseMessage(code)
        {
            Content = new StringContent(jsonResponse)
        };
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? "";
        return Task.FromResult(_responses.GetValueOrDefault(url)
            ?? new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}
