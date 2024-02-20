using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Data;

public class SuperAdminDbContext : IdentityDbContext
{
    public DbSet<AppRecord> AppRecords { get; set; }
    public DbSet<DbInfoRecord> DbInfoRecords { get; set; }
    
    public SuperAdminDbContext(DbContextOptions<SuperAdminDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }
}