namespace Hyprx.Platform.Environment;

public interface IEnviron : IEnumerable<KeyValuePair<string, string>>
{
    EnvResult Home { get; set; }

    EnvResult Path { get; set; }

    EnvResult User { get; set; }

    EnvResult SudoUser { get; set; }

    string? this[string name] { get; set; }

    void Add(string name, string value);

    bool TryAdd(string name, string value);

    EnvResult Get(string name);

    string GetString(string name, string defaultValue = "");

    bool Has(string name);

    void Set(string name, string value);

    bool TryGet(string name, out string value);

    void Remove(string name);

    IDictionary<string, string> ToDictionary();
}