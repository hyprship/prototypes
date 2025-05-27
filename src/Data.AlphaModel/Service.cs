using Hyprx.Data.AlphaModel;

namespace Hyprship.Data.AlphaModel;

public class Service
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public string Name { get; set; } = string.Empty;

    public int ServiceKindId { get; set; }

    public ServiceKind ServiceKind
    {
        get => (ServiceKind)this.ServiceKindId;
        set => this.ServiceKindId = (int)value;
    }

    public List<Context> Contexts { get; set; } = new List<Context>();

    public List<Secret> Secrets { get; set; } = new List<Secret>();

    public List<ConfigValue> ConfigValues { get; set; } = new List<ConfigValue>();
}