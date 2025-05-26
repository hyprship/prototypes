using System.Collections;
using System.Runtime.InteropServices;

namespace Hyprx.Platform.Environment;

public class Environ : IEnviron
{
    private readonly Dictionary<string, string> env = new(StringComparer.OrdinalIgnoreCase);

    public Environ()
    {
    }

    public Environ(Dictionary<string, string> env)
        => this.env = new Dictionary<string, string>(env, StringComparer.OrdinalIgnoreCase);

    public EnvResult Home
    {
        get => this.Get(EnvKeys.Home);
        set => this.Set(EnvKeys.Home, value.Value);
    }

    public EnvResult Path
    {
        get => this.Get(EnvKeys.Path);
        set => this.Set(EnvKeys.Path, value.Value);
    }

    public EnvResult User
    {
        get => this.Get(EnvKeys.User);
        set => this.Set(EnvKeys.User, value.Value);
    }

    public EnvResult SudoUser
    {
        get => this.Get(EnvKeys.SudoUser);
        set => this.Set(EnvKeys.SudoUser, value.Value);
    }

    public string? this[string name]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        => this.env.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();

    public void Add(string name, string value)
        => this.env.Add(name, value);

    public void AppendPath(string path)
    {
        var paths = this.SplitPath();
        if (Env.HasPath(paths, path))
            return;

        Array.Resize(ref paths, paths.Length + 1);
        paths[^1] = path;
        this.Set(EnvKeys.Path, Env.JoinPath(paths));
    }

    public EnvResult Get(string name)
        => new(name, this.GetValue(name));

    public string GetString(string name, string defaultValue = "")
        => this.GetValue(name) ?? defaultValue;

    public bool Has(string name)
        => this.env.ContainsKey(name);

    public bool HasPath(string path)
        => Env.HasPath(this.SplitPath(), path);

    public void PrependPath(string path)
    {
        var paths = this.SplitPath();
        if (Env.HasPath(paths, path))
            return;

        Array.Resize(ref paths, paths.Length + 1);
        for (var i = paths.Length - 1; i > 0; i--)
        {
            paths[i] = paths[i - 1];
        }

        paths[0] = path;
        this.Set(EnvKeys.Path, Env.JoinPath(paths));
    }

    public void Set(string name, string value)
        => this.env[name] = value;

    public string[] SplitPath()
        => Env.SplitPath(this.Get(EnvKeys.Path).OrDefault(string.Empty));

    public bool TryAdd(string name, string value)
        => this.env.TryAdd(name, value);

    public bool TryGet(string name, out string value)
    {
        var res = this.env.TryGetValue(name, out var v);
        v ??= string.Empty;
        value = v;
        return res;
    }

    public void Remove(string name)
        => this.env.Remove(name);

    public IDictionary<string, string> ToDictionary()
        => new Dictionary<string, string>(this.env, StringComparer.OrdinalIgnoreCase);

    private string? GetValue(string name)
    {
        if (this.env.TryGetValue(name, out var value))
            return value;

        return null;
    }
}