using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Data.MsSqlContext;

public class MsSqlSuperAdminDbContext : IdentityDbContext
{
    public DbSet<AppRecord> AppRecords { get; set; }
    public DbSet<DbInfoRecord> DbInfoRecords { get; set; }
    
    public MsSqlSuperAdminDbContext(DbContextOptions<MsSqlSuperAdminDbContext> options) : base(options)
    {
        Database.Migrate();
    }
}