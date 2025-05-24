using Microsoft.AspNetCore.Identity;

namespace Hyprship.Data.Model;

public class RoleClaim : IdentityRoleClaim<int>
{
    public RoleClaim()
        : base()
    {
    }
}