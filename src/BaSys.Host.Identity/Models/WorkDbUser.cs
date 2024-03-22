using Microsoft.AspNetCore.Identity;

namespace BaSys.Host.Identity.Models;

public class WorkDbUser : IdentityUser
{
    public string? DbName { get; set; }
}