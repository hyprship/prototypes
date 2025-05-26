namespace Hyprx.Platform.Process;

public class Command<TCommand> : ICommand
    where TCommand : Command<TCommand>
{
    protected Command(string file, CommandArgs args)
        : this(file)
    {
        this.Options.Args = args;
    }

    protected Command(string file)
    {
        this.Options = new CommandOptions()
        {
            File = file,
        };
    }

    public CommandOptions Options { get; set; }

    /// <summary>
    /// Gets or sets the input stream. This is only used when running the command. Spawning a command will
    /// not use this stream.
    /// </summary>
    public Stream? Input { get; set; }

    protected List<IDisposable> Disposables { get; } = [];

    public TCommand AddDisposable(IDisposable disposable)
    {
        this.Disposables.Add(disposable);
        return (TCommand)this;
    }

    public CommandPipe Pipe(CommandOptions command)
        => this.CreatePipe().Pipe(command);

    public CommandPipe Pipe(CommandArgs args)
        => this.CreatePipe().Pipe(args);

    public CommandPipe Pipe(ICommand command)
        => this.CreatePipe().Pipe(command);

    public virtual Output Output()
    {
        this.Options.Stdout = Stdio.Piped;
        this.Options.Stderr = Stdio.Piped;
        return this.Run();
    }

    public virtual ValueTask<Output> OutputAsync(CancellationToken cancellationToken = default)
    {
        this.Options.Stdout = Stdio.Piped;
        this.Options.Stderr = Stdio.Piped;
        return this.RunAsync(cancellationToken);
    }

    public virtual Output Run()
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

    public virtual async ValueTask<Output> RunAsync(CancellationToken cancellationToken = default)
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

    public virtual ChildProcess Spawn()
    {
        var process = new ChildProcess(this.Options);
        process.AddDisposables(this.Disposables);
        return process;
    }

    protected virtual CommandPipe CreatePipe()
    {
        var pipe = new CommandPipe();
        pipe.Pipe(this);
        return pipe;
    }
}