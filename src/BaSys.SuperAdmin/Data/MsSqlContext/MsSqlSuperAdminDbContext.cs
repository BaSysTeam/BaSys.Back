using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Data.MsSqlContext;

public class MsSqlSuperAdminDbContext : SuperAdminDbContext
{
    public MsSqlSuperAdminDbContext(DbContextOptions<MsSqlSuperAdminDbContext> options)
        : base(options)
    {
        base.Database.Migrate();
    }
}