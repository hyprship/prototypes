using System.Diagnostics.CodeAnalysis;

namespace Hyprx.Platform.Environment;

internal static class EnvExtensions
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
        => string.IsNullOrWhiteSpace(value);
}