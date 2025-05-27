namespace Hyprship.Data.AlphaModel;

public class Tag
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public HashSet<TagValue> Values { get; set; } = new HashSet<TagValue>();

    public override string ToString()
    {
        return $"{this.Name}";
    }
}