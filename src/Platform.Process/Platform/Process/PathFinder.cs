using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Hyprx.Platform.Process;

public class PathFinder
{
    private readonly ConcurrentDictionary<string, PathHint> entries = new(StringComparer.OrdinalIgnoreCase);

    public static PathFinder Default { get; } = new();

    public PathHint? this[string name]
    {
        get => this.entries.TryGetValue(name, out var entry) ? entry : null;
        set
        {
            if (value is null)
                this.entries.TryRemove(name, out _);
            else
                this.entries[name] = value;
        }
    }

    public void Register(string name, PathHint entry)
    {
        this.entries[name] = entry;
        if (entry.Variable.IsNullOrWhiteSpace())
        {
            entry.Variable = name.ScreamingSnakeCase() + "_PATH";
        }
    }

    public void Register(string name, Func<PathHint> factory)
    {
        if (!this.entries.TryGetValue(name, out _))
        {
            this.entries[name] = factory();
        }
    }

    public void RegisterOrUpdate(string name, Action<PathHint> update)
    {
        if (!this.entries.TryGetValue(name, out var entry))
        {
            entry = new PathHint(name);
            this.Register(name, entry);
        }

        update(entry);
    }

    public void Update(string name, Action<PathHint> update)
    {
        if (this.entries.TryGetValue(name, out var entry))
        {
            update(entry);
        }
    }

    public bool Has(string name)
    {
        return this.entries.ContainsKey(name);
    }

    public string FindOrThrow(string name)
    {
        var path = this.Find(name);
        if (path is null)
            throw new FileNotFoundException($"Could not find {name} on the PATH.");

        return path;
    }

    public string? Find(string name)
    {
#if NET5_0_OR_GREATER
        if (Path.IsPathFullyQualified(name))
            return name;
#else
        if (Path.IsPathRooted(name))
            return name;
#endif
        var entry = this[name];
        if (entry is null)
        {
            entry = new PathHint(name);
            this.Register(name, entry);
        }

        if (!entry.Variable.IsNullOrWhiteSpace())
        {
            var cached = !entry.CachedPath.IsNullOrWhiteSpace();
            var envPath = Env.Get(entry.Variable);
            if (envPath.HasValue)
            {
                if (cached && envPath == entry.CachedPath)
                    return envPath;

                var path = Env.ExpandString(envPath);
                if (path.Length > 0)
                {
                    path = Path.GetFullPath(path);
                    if (cached && entry.CachedPath == envPath)
                        return envPath;

                    var tmp = Command.Which(path);
                    if (tmp is not null)
                    {
                        entry.CachedPath = tmp;
                        return tmp;
                    }
                }
            }
        }

        if (!entry.CachedPath.IsNullOrWhiteSpace())
            return entry.CachedPath;

        var exe = entry.Executable ?? name;
        exe = Command.Which(exe);
        if (exe is not null)
        {
            entry.Executable = Path.GetFileName(exe);
            entry.CachedPath = exe;
            return exe;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            foreach (var attempt in entry.Windows)
            {
                exe = attempt;
                exe = Env.ExpandString(exe);
                exe = Command.Which(exe);
                if (exe is null)
                {
                    continue;
                }

                entry.Executable = Path.GetFileName(exe);
                entry.CachedPath = exe;
                return exe;
            }

            return null;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            foreach (var attempt in entry.Darwin)
            {
                exe = attempt;
                exe = Env.ExpandString(exe);
                exe = Command.Which(exe);
                if (exe is null)
                {
                    continue;
                }

                entry.Executable = Path.GetFileName(exe);
                entry.CachedPath = exe;
                return exe;
            }
        }

        foreach (var attempt in entry.Linux)
        {
            exe = attempt;
            exe = Env.ExpandString(exe);
            exe = Command.Which(exe);
            if (exe is null)
            {
                continue;
            }

            entry.Executable = Path.GetFileName(exe);
            entry.CachedPath = exe;
            return exe;
        }

        return null;
    }
}