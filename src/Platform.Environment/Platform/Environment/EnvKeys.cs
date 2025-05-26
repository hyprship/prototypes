using System.Runtime.InteropServices;

namespace Hyprx.Platform.Environment;

public static class EnvKeys
{
    public static string Home => IsWindows ? "USERPROFILE" : "HOME";

    public static string Path => IsWindows ? "Path" : "PATH";

    public static string User => IsWindows ? "USERNAME" : "USER";

    public static string SudoUser => "SUDO_USER";

    public static string Temp => "TEMP";

    private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}