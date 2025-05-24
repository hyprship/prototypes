using Microsoft.AspNetCore.Identity;

namespace Hyprship.Data.Model;

public class UserRole : IdentityUserRole<int>
{
    public UserRole()
        : base()
    {
    }
}