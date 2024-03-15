using BaSys.SuperAdmin.DAL.Models;
using BaSys.SuperAdmin.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.DAL;

public class SuperAdminDbContext : IdentityDbContext<SaDbUser, SaDbRole, string>
{
    public DbSet<AppRecord> AppRecords { get; set; }
    public DbSet<DbInfoRecord> DbInfoRecords { get; set; }
    
    public SuperAdminDbContext(DbContextOptions options) : base(options)
    {
    }
}