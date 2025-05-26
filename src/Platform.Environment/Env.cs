using System.Collections;
using System.Runtime.InteropServices;

using Hyprx.Platform.Environment;

using static System.Environment;

namespace Hyprx;

public static partial class Env
{
    public static EnvResult Home
    {
        get => Get(EnvKeys.Home);
        set => Set(EnvKeys.Home, value.Value);
    }

    public static EnvResult Path
    {
        get => Get(EnvKeys.Path);
        set => Set(EnvKeys.Path, value.Value);
    }

    public static EnvResult User
    {
        get => Get(EnvKeys.User);
        set => Set(EnvKeys.User, value.Value);
    }

    public static EnvResult SudoUser
    {
        get => Get(EnvKeys.SudoUser);
        set => Set(EnvKeys.SudoUser, value.Value);
    }

    public static string Cwd
    {
        get => System.Environment.CurrentDirectory;
        set => System.Environment.CurrentDirectory = value;
    }

    private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public static void Add(string name, string value)
    {
        if (Has(name))
            throw new EnvironmentException($"Environment variable {name} already exists.");

        Set(name, value);
    }

    public static bool TryAdd(string name, string value)
    {
        if (Has(name))
            return false;

        Set(name, value);
        return true;
    }

    public static EnvResult Get(string name)
        => new EnvResult(name, GetEnvironmentVariable(name));

    public static EnvResult Get(string name, EnvironmentVariableTarget target)
        => new EnvResult(name, GetEnvironmentVariable(name, target));

    public static string? GetString(string name)
        => GetEnvironmentVariable(name);

    public static string? GetString(string name, EnvironmentVariableTarget target)
        => GetEnvironmentVariable(name, target);

    public static string GetString(string name, string defaultValue)
        => GetEnvironmentVariable(name) ?? defaultValue;

    public static string GetString(string name, string defaultValue, EnvironmentVariableTarget target)
        => GetEnvironmentVariable(name, target) ?? defaultValue;

    public static bool Has(string name)
        => GetEnvironmentVariable(name) is not null;

    public static bool Has(string name, EnvironmentVariableTarget target)
        => GetEnvironmentVariable(name, target) is not null;

    public static void Set(IDictionary<string, string> values)
    {
        foreach (var kvp in values)
        {
            if (kvp.Key.IsNullOrWhiteSpace())
                continue;

            if (kvp.Value.IsNullOrWhiteSpace())
                continue;

            SetEnvironmentVariable(kvp.Key, kvp.Value);
        }
    }

    public static void Set(string name, string value)
        => SetEnvironmentVariable(name, value);

    public static void Set(string name, string value, EnvironmentVariableTarget target)
        => SetEnvironmentVariable(name, value, target);

    public static void AppendPath(string path)
    {
        var paths = SplitPath();
        if (HasPath(paths, path))
            return;

        Array.Resize(ref paths, paths.Length + 1);
        paths[^1] = path;
        Set(EnvKeys.Path, JoinPath(paths));
    }

    public static void AppendPath(string path, EnvironmentVariableTarget target)
    {
        var paths = SplitPath(GetString(EnvKeys.Path, target) ?? string.Empty);
        if (HasPath(paths, path))
            return;

        Array.Resize(ref paths, paths.Length + 1);
        paths[^1] = path;
        Set(EnvKeys.Path, JoinPath(paths), target);
    }

    public static void PrependPath(string path)
    {
        var paths = SplitPath();
        if (HasPath(paths, path))
            return;

        Array.Resize(ref paths, paths.Length + 1);
        for (var i = paths.Length - 1; i > 0; i--)
        {
            paths[i] = paths[i - 1];
        }

        paths[0] = path;
        Set(EnvKeys.Path, JoinPath(paths));
    }

    public static void PrependPath(string path, EnvironmentVariableTarget target)
    {
        var paths = SplitPath(GetString(EnvKeys.Path, target) ?? string.Empty);
        if (HasPath(paths, path))
            return;

        Array.Resize(ref paths, paths.Length + 1);
        for (var i = paths.Length - 1; i > 0; i--)
        {
            paths[i] = paths[i - 1];
        }

        paths[0] = path;
        Set(EnvKeys.Path, JoinPath(paths), target);
    }

    public static bool HasPath(string path)
        => HasPath(SplitPath(), path);

    public static bool HasPath(string[] paths, string path)
    {
        if (IsWindows)
        {
            foreach (var p in paths)
            {
                if (string.Equals(p, path, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        foreach (var p in paths)
        {
            if (string.Equals(p, path, StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    public static string[] SplitPath()
        => SplitPath(Path.Value);

    public static string[] SplitPath(string path)
        => path.Split(System.IO.Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

    public static string JoinPath(params string[] paths)
#if NET5_0_OR_GREATER
        => string.Join(System.IO.Path.PathSeparator, paths);
#else
        => string.Join(System.IO.Path.PathSeparator.ToString(), paths);
#endif

    public static void Remove(string name)
        => SetEnvironmentVariable(name, null);

    public static void Remove(string name, EnvironmentVariableTarget target)
        => SetEnvironmentVariable(name, null, target);

    public static bool TryGet(string name, out string value)
    {
        var v = GetEnvironmentVariable(name);
        value = v ?? string.Empty;
        return v is not null;
    }

    public static IEnumerable<KeyValuePair<string, string>> Enumerate()
    {
        foreach (DictionaryEntry entry in GetEnvironmentVariables())
        {
            if (entry.Key is not string key || key.IsNullOrWhiteSpace())
                continue;

            if (entry.Value is not string value || value.IsNullOrWhiteSpace())
                continue;

            yield return new KeyValuePair<string, string>(key, value);
        }
    }

    public static Dictionary<string, string> ToDictionary()
    {
        var env = new Dictionary<string, string>();
        foreach (var kvp in Enumerate())
        {
            env.Add(kvp.Key, kvp.Value);
        }

        return env;
    }
}