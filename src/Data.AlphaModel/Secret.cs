namespace Hyprship.Data.AlphaModel;

public class Secret
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public int ContextId { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string? ContentType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; } = null;

    public HashSet<TagValue> Tags { get; set; } = new HashSet<TagValue>();
}