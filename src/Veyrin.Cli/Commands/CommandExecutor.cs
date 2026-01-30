namespace Veyrin.Cli.Commands;

public class CommandExecutor
{
    private readonly Dictionary<string, Func<CommandArgs, Task>> _commands = new(StringComparer.OrdinalIgnoreCase);
    private Func<CommandArgs, Task>? _defaultAction;

    public CommandExecutor Register(string verb, Func<CommandArgs, Task> action)
    {
        _commands[verb] = action;
        return this;
    }

    public CommandExecutor OnDefault(Func<CommandArgs, Task> action)
    {
        _defaultAction = action;
        return this;
    }

    public async Task ExecuteAsync(string[] args)
    {
        var cmdArgs = new CommandArgs(args);
        var verb = cmdArgs.GetArgument(0);

        if (verb != null && _commands.TryGetValue(verb, out var action))
            await action(cmdArgs);
        else if (_defaultAction != null)
            await _defaultAction(cmdArgs);
        else
            Console.WriteLine($"Unknown command: {verb ?? "None"}");
    }
}
