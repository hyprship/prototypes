namespace Hyprx.Platform.Process;

public interface ICommand
{
    CommandOptions Options { get; }

    CommandPipe Pipe(CommandOptions command);

    CommandPipe Pipe(CommandArgs args);

    CommandPipe Pipe(ICommand command);

    Output Output();

    ValueTask<Output> OutputAsync(CancellationToken cancellationToken = default);

    Output Run();

    ValueTask<Output> RunAsync(CancellationToken cancellationToken = default);

    ChildProcess Spawn();
}

public static class CommandExtensions
{
    public static ICommand WithArgs(this ICommand cmd, CommandArgs args)
    {
        cmd.Options.Args = args;
        return cmd;
    }

    public static ICommand AddArg(this ICommand cmd, string arg)
    {
        cmd.Options.Args.Add(arg);
        return cmd;
    }

    public static ICommand AddArgs(this ICommand cmd, CommandArgs args)
    {
        cmd.Options.Args.AddRange(args);
        return cmd;
    }

    public static ICommand WithCwd(this ICommand cmd, string cwd)
    {
        cmd.Options.Cwd = cwd;
        return cmd;
    }

    public static ICommand WithEnv(this ICommand cmd, IEnumerable<KeyValuePair<string, string>> env)
    {
        var dictionary = new Dictionary<string, string?>();
        foreach (var pair in env)
        {
            dictionary.Add(pair.Key, pair.Value);
        }

        cmd.Options.Env = dictionary;
        return cmd;
    }

    public static ICommand SetEnv(this ICommand cmd, string name, string value)
    {
        cmd.Options.Env ??= new Dictionary<string, string?>();
        cmd.Options.Env[name] = value;
        return cmd;
    }

    public static ICommand SetEnv(this ICommand cmd, IEnumerable<KeyValuePair<string, string>> env)
    {
        cmd.Options.Env ??= new Dictionary<string, string?>();
        foreach (var pair in env)
        {
            cmd.Options.Env.Add(pair.Key, pair.Value);
        }

        return cmd;
    }

    public static ICommand WithStdout(this ICommand cmd, Stdio stdio)
    {
        cmd.Options.Stdout = stdio;
        return cmd;
    }

    public static ICommand WithStderr(this ICommand cmd, Stdio stdio)
    {
        cmd.Options.Stderr = stdio;
        return cmd;
    }

    public static ICommand WithStdin(this ICommand cmd, Stdio stdio)
    {
        cmd.Options.Stdin = stdio;
        return cmd;
    }

    public static ICommand WithStdio(this ICommand cmd, Stdio stdio)
    {
        cmd.Options.WithStdio(stdio);
        return cmd;
    }

    public static ICommand AsPipedOutput(this ICommand cmd)
    {
        cmd.Options.Stdout = Stdio.Piped;
        cmd.Options.Stderr = Stdio.Piped;
        return cmd;
    }

    public static ICommand AsInheritOutput(this ICommand cmd)
    {
        cmd.Options.Stdout = Stdio.Inherit;
        cmd.Options.Stderr = Stdio.Inherit;
        return cmd;
    }

    public static ICommand AsNullOutput(this ICommand cmd)
    {
        cmd.Options.Stdout = Stdio.Null;
        cmd.Options.Stderr = Stdio.Null;
        return cmd;
    }
}