using BaSys.Host.DAL.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.DAL;

public class ApplicationDbContext : IdentityDbContext<WorkDbUser, WorkDbRole, string>
{
    // Add DbSets here
    
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<WorkDbUser>()
            .Property(x => x.DbName)
            .HasMaxLength(50);
    }
}