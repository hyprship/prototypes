namespace Hyprship.Data.AlphaModel;

public class ComputeNode
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public string Name { get; set; } = string.Empty;

    public string Host { get; set; } = string.Empty;

    public string? Description { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? Facts { get; set; } = null;

    public int PlatformKindId { get; set; } = (int)PlatformKind.Unknown;

    public bool IsDeleted { get; set; } = false;

    public int? RemoteCredentialId { get; set; }

    public RemoteCredential? RemoteCredential { get; set; } = null;
}