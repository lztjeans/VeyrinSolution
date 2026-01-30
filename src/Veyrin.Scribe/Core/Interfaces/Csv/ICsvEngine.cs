
/// <summary>
/// 定義 CSV 操作引擎的共用介面。
/// 可由 CsvHelper 等具體引擎實作。
/// </summary>
public interface ICsvEngine
{
    ICsvEngine CreateCsv();
    ICsvEngine LoadCsv(string path);
    T? Read<T>() where T : class;
    string ReadText();
    ICsvEngine SaveToFile(string path);
    void Write<T>(T content) where T : class;
    ICsvEngine WriteText(string content);
}