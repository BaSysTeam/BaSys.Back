using Microsoft.AspNetCore.Identity;

namespace BaSys.Host.Identity.Models;

public class WorkDbRole : IdentityRole
{
    public WorkDbRole()
    {
    }
    
    public WorkDbRole(string roleName) : base(roleName)
    {
    }
}