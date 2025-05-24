using Microsoft.AspNetCore.Identity;

namespace Hyprship.Data.Model;

public class UserLogin : IdentityUserLogin<int>
{
    public UserLogin()
        : base()
    {
    }
}