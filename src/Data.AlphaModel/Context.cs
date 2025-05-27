using Hyprx.Data.AlphaModel;

namespace Hyprship.Data.AlphaModel;

public class Context
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public string Name { get; set; } = string.Empty;

    public int? ComputeNodeId { get; set; }

    public ComputeNode? ComputeNode { get; set; } = null;

    public List<Secret> Secrets { get; set; } = new List<Secret>();

    public List<ConfigValue> ConfigValues { get; set; } = new List<ConfigValue>();
}