using Microsoft.AspNetCore.Identity;

namespace Hyprship.Data.Model;

public class User : IdentityUser<int>
{
    public Guid SyncId { get; set; } = Guid.CreateVersion7();
}