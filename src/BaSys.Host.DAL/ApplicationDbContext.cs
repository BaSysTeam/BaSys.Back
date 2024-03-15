using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.DAL;

public class ApplicationDbContext : IdentityDbContext
{
    // Add DbSets here
    
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }
}