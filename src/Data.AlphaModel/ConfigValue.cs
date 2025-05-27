namespace Hyprx.Data.AlphaModel;

public class ConfigValue
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public int ContextId { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string? TypeHint { get; set; }

    public override string ToString()
    {
        return $"{Key}={Value}";
    }
}