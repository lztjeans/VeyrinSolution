

public class DataNotFoundException : Exception
{
    public DataNotFoundException(string message) : base($"{message} isn't found!") { }
}