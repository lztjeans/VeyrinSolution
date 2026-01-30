namespace Veyrin.Cli.Terminal;

public class Spinner : IDisposable
{
    private readonly int _left;
    private readonly int _top;
    private readonly string[] _frames = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];
    private int _currentFrame = 0;
    private bool _active = true;

    public Spinner()
    {
        (_left, _top) = Console.GetCursorPosition();
        Console.CursorVisible = false;
        Task.Run(Animate);
    }

    private async Task Animate()
    {
        while (_active)
        {
            Console.SetCursorPosition(_left, _top);
            ConsoleWriter.Write(_frames[_currentFrame], AnsiCodes.Cyan);
            _currentFrame = (_currentFrame + 1) % _frames.Length;
            await Task.Delay(100);
        }
    }

    public void Stop(string finalMessage = "Done!")
    {
        _active = false;
        Console.SetCursorPosition(_left, _top);
        ConsoleWriter.Success(finalMessage);
        Console.CursorVisible = true;
    }

    public void Dispose() => Stop();
}
