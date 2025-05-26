using System.Diagnostics.CodeAnalysis;

namespace Hyprx.Platform.Process;

internal static class InternalExecExtensions
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
        => string.IsNullOrWhiteSpace(value);
}