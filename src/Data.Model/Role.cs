using Microsoft.AspNetCore.Identity;

namespace Hyprship.Data.Model;

public class Role : IdentityRole<int>
{
    public Role()
        : base()
    {
    }

    public Role(string roleName)
        : base(roleName)
    {
    }

    public Guid SyncId { get; set; } = Guid.CreateVersion7();
}