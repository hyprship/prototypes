using Hyprx.ComponentModel.Annotations;

namespace Hyprship.Data.AlphaModel;

public class RemoteCredential
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid SyncId { get; set; } = Guid.CreateVersion7();

    public int TypeId { get; set; } = (int)RemoteCredentialType.SshKeyPair;

    public string Username { get; set; } = string.Empty;

    public int Port { get; set; } = 22;

    public int SecretId { get; set; } = 0;

    public Secret? Secret { get; set; } = null;

    public int? CertificateId { get; set; } = null;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; } = null;
}