namespace Hyprx;

public class EnvExpandOptions
{
    public bool WindowsExpansion { get; set; } = true;

    public bool UnixExpansion { get; set; } = true;

    public bool UnixAssignment { get; set; } = true;

    public bool UnixCustomErrorMessage { get; set; } = true;

    public bool UnixArgsExpansion { get; set; }

    public Func<string, string?>? GetVariable { get; set; }

    public Action<string, string>? SetVariable { get; set; }
}