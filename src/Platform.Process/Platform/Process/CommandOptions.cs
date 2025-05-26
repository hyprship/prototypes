using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace Hyprx.Platform.Process;

public class CommandOptions
{
    public CommandOptions()
    {
        this.Args = [];
    }

    public CommandOptions(CommandArgs args)
    {
        this.File = args.Shift();
        this.Args = args;
    }

    public string File { get; set; } = string.Empty;

    public CommandArgs Args { get; set; }

    public string? Cwd { get; set; }

    public IDictionary<string, string?>? Env { get; set; }

    public Stdio Stdout { get; set; }

    public Stdio Stderr { get; set; }

    public Stdio Stdin { get; set; }

    public string? User { get; set; }

    public string? Verb { get; set; }

    [SupportedOSPlatform("windows")]
    [CLSCompliant(false)]
    public SecureString? Password { get; set; }

    [SupportedOSPlatform("windows")]
    public string? PasswordInClearText { get; set; }

    [SupportedOSPlatform("windows")]
    public string? Domain { get; set; }

    public bool LoadUserProfile { get; set; } = false;

    public bool CreateNoWindow { get; set; } = false;

    public bool UseShellExecute { get; set; } = false;

    public CommandOptions WithStdio(Stdio stdio)
    {
        this.Stdout = stdio;
        this.Stderr = stdio;
        this.Stdin = stdio;

        return this;
    }

    public ProcessStartInfo ToStartInfo()
    {
        var si = new ProcessStartInfo();
        return this.ToStartInfo(si);
    }

    public ProcessStartInfo ToStartInfo(ProcessStartInfo startInfo)
    {
        var si = startInfo;
        si.FileName = this.File;
        si.CreateNoWindow = this.CreateNoWindow;
        si.UseShellExecute = this.UseShellExecute;
        si.RedirectStandardInput = this.Stdin == Stdio.Piped;

        var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        if (windows)
            si.LoadUserProfile = startInfo.LoadUserProfile;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !this.User.IsNullOrWhiteSpace())
        {
            si.UserName = this.User;

            if (startInfo.Password is not null)
            {
                si.Password = startInfo.Password;
            }
            else if (!startInfo.PasswordInClearText.IsNullOrWhiteSpace())
            {
                si.PasswordInClearText = startInfo.PasswordInClearText;
            }

            if (!startInfo.Domain.IsNullOrWhiteSpace())
            {
                si.Domain = startInfo.Domain;
            }
        }

#if NET5_0_OR_GREATER
        foreach (var arg in this.Args)
        {
            si.ArgumentList.Add(arg);
        }
#else
        si.Arguments = this.Args.ToString();
#endif

        if (!this.Cwd.IsNullOrWhiteSpace())
            si.WorkingDirectory = this.Cwd;

        if (this.Env is not null)
        {
            foreach (var kvp in this.Env)
            {
                si.Environment[kvp.Key] = kvp.Value;
            }
        }

        si.RedirectStandardOutput = this.Stdout != Stdio.Inherit;
        si.RedirectStandardError = this.Stderr != Stdio.Inherit;
        si.RedirectStandardInput = this.Stdin != Stdio.Inherit;
        if (si.RedirectStandardError || si.RedirectStandardOutput)
        {
            si.CreateNoWindow = true;
            si.UseShellExecute = false;
        }

        return si;
    }
}