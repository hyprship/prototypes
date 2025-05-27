using System.ComponentModel.DataAnnotations.Schema;

namespace Hyprship.Data.AlphaModel;

public class TagValue
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public int TagId { get; set; }

    public string Value { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Tag? Tag { get; set; } = null!;

    [NotMapped]
    public string Name
    {
        get => this.Tag?.Name ?? string.Empty;
    }

    [NotMapped]
    public string Slug
    {
        get => this.Tag?.Slug ?? string.Empty;
    }

    public override string ToString()
    {
        return $"{this.Value}";
    }
}