using Hyprx.ComponentModel.Annotations;

namespace Hyprship.Data.AlphaModel;

public class Certificate
{
    public int Id { get; set; }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public string Name { get; set; } = string.Empty;

    public string PublicKey { get; set; } = string.Empty;

    public bool HasPrivateKey { get; set; } = false;

    [Encrypt]
    public string? PrivateKey { get; set; } = string.Empty;

    public string? Thumbprint { get; set; } = null;

    public string? Subject { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ValidAt { get; set; } = null;

    public DateTime? ExpiresAt { get; set; } = null;

    [Encrypt]
    public string? Secret { get; set; } = null;
}