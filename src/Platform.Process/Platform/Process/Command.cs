namespace Hyprx.Platform.Process;

public partial class Command : ICommand
{
    public Command(CommandArgs args)
        : this(new CommandOptions(args))
    {
    }

    public Command(CommandOptions options)
    {
        this.Options = options;
    }

    public CommandOptions Options { get; }

    /// <summary>
    /// Gets or sets the input stream. This is only used when running the command. Spawning a command will
    /// not use this stream.
    /// </summary>
    public Stream? Input { get; set; }

    protected List<IDisposable> Disposables { get; } = [];

    public static Command New(CommandArgs args)
    {
        return new Command(args);
    }

    public static Command New(CommandOptions options)
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

    public ICommand AddDisposable(IDisposable disposable)
    {
        this.Disposables.Add(disposable);
        return this;
    }

    public CommandPipe Pipe(CommandOptions command)
    {
        var pipe = new CommandPipe();
        pipe.Pipe(this);
        pipe.Pipe(command);
        return pipe;
    }

    public CommandPipe Pipe(CommandArgs args)
    {
        var pipe = new CommandPipe();
        pipe.Pipe(this);
        pipe.Pipe(args);
        return pipe;
    }

    public CommandPipe Pipe(ICommand command)
    {
        var pipe = new CommandPipe();
        pipe.Pipe(this);
        pipe.Pipe(command);
        return pipe;
    }

    public Output Output()
    {
        this.Options.Stdout = Stdio.Piped;
        this.Options.Stderr = Stdio.Piped;
        return this.Run();
    }

    public ValueTask<Output> OutputAsync(CancellationToken cancellationToken = default)
    {
        this.Options.Stdout = Stdio.Piped;
        this.Options.Stderr = Stdio.Piped;
        return this.RunAsync(cancellationToken);
    }

    public Output Run()
    {
        try
        {
            var hasInput = this.Input is not null;
            if (hasInput)
            {
                this.Options.Stdin = Stdio.Piped;
            }

            using var process = new ChildProcess(this.Options);
            if (hasInput)
            {
                process.PipeFrom(this.Input!);
            }

            process.AddDisposables(this.Disposables);
            return process.WaitForOutput();
        }
        catch (Exception ex)
        {
            return new Output(
                this.Options.File,
                ex);
        }
    }

    public async ValueTask<Output> RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var hasInput = this.Input is not null;
            if (hasInput)
            {
                this.Options.Stdin = Stdio.Piped;
            }

            using var process = new ChildProcess(this.Options);
            if (hasInput)
            {
                process.PipeFrom(this.Input!);
            }

            process.AddDisposables(this.Disposables);
            var output = await process.WaitForOutputAsync(cancellationToken);
            return output;
        }
        catch (Exception ex)
        {
            return new Output(
                this.Options.File,
                ex);
        }
    }

    public ChildProcess Spawn()
    {
        var process = new ChildProcess(this.Options);
        process.AddDisposables(this.Disposables);
        return process;
    }
}