using CsvHelper;
using System.Globalization;
using System.Text;
using Veyrin.Scribe.Core.Interfaces;

namespace Veyrin.Scribe.Text.Csv;

public class CsvEngine : ICsvEngine, IEngine
{
    private readonly List<string[]> _rows = [];

    /// <summary>
    /// 建立新的 CSV，清空現有資料
    /// </summary>
    public ICsvEngine CreateCsv()
    {
        _rows.Clear();
        return this;
    }

    /// <summary>
    /// 從 CSV 檔案讀取內容
    /// </summary>
    public ICsvEngine LoadCsv(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("CSV 檔案不存在", path);

        _rows.Clear();

        using var reader = new StreamReader(path, Encoding.UTF8);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        while (csv.Read())
        {
            var record = new List<string>();
            for (int i = 0; csv.TryGetField<string>(i, out var field); i++)
                record.Add(field ?? "");

            _rows.Add([.. record]);
        }

        return this;
    }

    /// <summary>
    /// 泛型讀取單筆物件（第一筆）
    /// </summary>
    public T? Read<T>() where T : class
    {
        var list = ReadAll<T>();
        if (list.Count > 0)
            return list[0];
        return default;
    }

    /// <summary>
    /// 泛型讀取全部物件
    /// </summary>
    public List<T> ReadAll<T>() where T : class
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // 將內部 _rows 轉回 CSV 格式
        foreach (var row in _rows)
        {
            foreach (var field in row)
                csvWriter.WriteField(field);
            csvWriter.NextRecord();
        }
        writer.Flush();
        memoryStream.Position = 0;

        using var reader = new StreamReader(memoryStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = new List<T>();
        records.AddRange(csv.GetRecords<T>());
        return records;
    }

    public string ReadText()
    {
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        foreach (var row in _rows)
        {
            foreach (var field in row)
                csv.WriteField(field);
            csv.NextRecord();
        }

        return writer.ToString();
    }

    /// <summary>
    /// 將 CSV 儲存為 byte[]
    /// </summary>
    public byte[] SaveToByteArray()
    {
        using var stream = SaveToStream();
        return stream.ToArray();
    }

    /// <summary>
    /// 將 CSV 儲存為檔案
    /// </summary>
    public ICsvEngine SaveToFile(string path)
    {
        using var writer = new StreamWriter(path, false, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        foreach (var row in _rows)
        {
            foreach (var field in row)
                csv.WriteField(field);
            csv.NextRecord();
        }

        return this;
    }

    /// <summary>
    /// 將 CSV 內容轉成 MemoryStream
    /// </summary>
    public MemoryStream SaveToStream()
    {
        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            foreach (var row in _rows)
            {
                foreach (var field in row)
                    csv.WriteField(field);
                csv.NextRecord();
            }
            writer.Flush();
        }
        stream.Position = 0;
        return stream;
    }

    /// <summary>
    /// 泛型寫入物件或列表
    /// </summary>
    public void Write<T>(T content) where T : class
    {
        if (content == null) return;

        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        if (content is IEnumerable<T> list)
        {
            csv.WriteRecords(list);
        }
        else
        {
            csv.WriteRecord(content);
            csv.NextRecord();
        }

        writer.Flush();
        memoryStream.Position = 0;

        using var reader = new StreamReader(memoryStream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line != null)
                _rows.Add(line.Split(',')); // 存成內部資料結構
        }
    }

    /// <summary>
    /// 手動新增一行 CSV 文字（逗號分隔）
    /// </summary>
    public ICsvEngine WriteText(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return this;

        var fields = content.Split(',');
        _rows.Add(fields);
        return this;
    }
}