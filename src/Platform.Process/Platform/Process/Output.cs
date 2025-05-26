namespace Hyprx.Platform.Process;

public readonly struct Output
{
    public Output()
    {
        this.ExitCode = 0;
        this.FileName = string.Empty;
        this.Stdout = [];
        this.Stderr = [];
        this.StartTime = DateTime.MinValue;
        this.ExitTime = DateTime.MinValue;
    }

    public Output(
        string fileName,
        int exitCode,
        byte[]? stdout = null,
        byte[]? stderr = null,
        DateTime? startTime = null,
        DateTime? exitTime = null)
    {
        this.FileName = fileName;
        this.ExitCode = exitCode;
        this.Stdout = stdout ?? [];
        this.Stderr = stderr ?? [];
        this.StartTime = startTime ?? DateTime.MinValue;
        this.ExitTime = exitTime ?? DateTime.MinValue;
        this.Exception = null;
    }

    public Output(
        string fileName,
        Exception exception,
        DateTime? startTime = null,
        DateTime? exitTime = null)
    {
        this.FileName = fileName;
        this.Exception = exception;
        this.ExitCode = -1;
        this.Stdout = [];
        this.Stderr = [];
        this.StartTime = startTime ?? DateTime.MinValue;
        this.ExitTime = exitTime ?? DateTime.MinValue;
    }

    public int ExitCode { get; }

    public Exception? Exception { get; }

    public string FileName { get; }

    public byte[] Stdout { get; }

    public byte[] Stderr { get; }

    public DateTime StartTime { get; }

    public DateTime ExitTime { get; }
}