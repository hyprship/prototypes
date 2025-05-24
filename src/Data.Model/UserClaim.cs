using Microsoft.AspNetCore.Identity;

namespace Hyprship.Data.Model;

public class UserClaim : IdentityUserClaim<int>
{
    public UserClaim()
        : base()
    {
    }
}