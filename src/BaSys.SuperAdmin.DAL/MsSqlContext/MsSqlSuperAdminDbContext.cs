using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.DAL.MsSqlContext;

public class MsSqlSuperAdminDbContext : SuperAdminDbContext
{
    public MsSqlSuperAdminDbContext(DbContextOptions<MsSqlSuperAdminDbContext> options)
        : base(options)
    {
        // base.Database.Migrate();
    }
}