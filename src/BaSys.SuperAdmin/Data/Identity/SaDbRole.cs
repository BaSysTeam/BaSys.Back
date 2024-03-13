using Microsoft.AspNetCore.Identity;

namespace BaSys.SuperAdmin.Data.Identity;

public class SaDbRole : IdentityRole
{
    public SaDbRole()
    {
    }
    
    public SaDbRole(string roleName) : base(roleName)
    {
    }
}