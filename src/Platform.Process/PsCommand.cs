using Hyprx.Platform.Process;

namespace Hyprx;

public static class PsCommand
{
    public static Command NewCommand(CommandArgs args)
    {
        return new Command(args);
    }

    public static Command NewCommand(CommandOptions options)
    {
        return new Command(options);
    }

    public static Output Output(CommandArgs args)
    {
        return new Command(args).Output();
    }

    public static Output Output(CommandOptions options)
    {
        return new Command(options).Output();
    }

    public static ValueTask<Output> OutputAsync(CommandArgs args, CancellationToken cancellationToken = default)
    {
        return new Command(args).OutputAsync(cancellationToken);
    }

    public static ValueTask<Output> OutputAsync(CommandOptions options, CancellationToken cancellationToken = default)
    {
        return new Command(options).OutputAsync(cancellationToken);
    }

    public static Output Run(CommandArgs args)
    {
        return new Command(args).Run();
    }

    public static Output Run(CommandOptions options)
    {
        return new Command(options).Run();
    }

    public static ValueTask<Output> RunAsync(CommandArgs args, CancellationToken cancellationToken = default)
    {
        return new Command(args).RunAsync(cancellationToken);
    }

    public static ValueTask<Output> RunAsync(CommandOptions options, CancellationToken cancellationToken = default)
    {
        return new Command(options).RunAsync(cancellationToken);
    }

    public static ChildProcess Spawn(CommandArgs args)
    {
        return new Command(args).Spawn();
    }

    public static ChildProcess Spawn(CommandOptions options)
    {
        return new Command(options).Spawn();
    }
}