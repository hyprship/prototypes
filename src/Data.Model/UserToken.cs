using Microsoft.AspNetCore.Identity;

namespace Hyprship.Data.Model;

public class UserToken : IdentityUserToken<int>
{
    public UserToken()
        : base()
    {
    }
}