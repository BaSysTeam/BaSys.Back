using Microsoft.AspNetCore.Identity;

namespace BaSys.Host.DAL.Identity;

public class WorkDbUser : IdentityUser
{
    public string? DbName { get; set; }
}