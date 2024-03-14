using BaSys.SuperAdmin.Data.Identity;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Data;

public class SuperAdminDbContext : IdentityDbContext<SaDbUser, SaDbRole, string>
{
    public DbSet<AppRecord> AppRecords { get; set; }
    public DbSet<DbInfoRecord> DbInfoRecords { get; set; }
    
    public SuperAdminDbContext(DbContextOptions options) : base(options)
    {
    }
}