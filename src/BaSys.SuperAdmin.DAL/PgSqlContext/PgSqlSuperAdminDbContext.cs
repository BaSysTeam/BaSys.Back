using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.DAL.PgSqlContext;

public class PgSqlSuperAdminDbContext : SuperAdminDbContext
{
    public PgSqlSuperAdminDbContext(DbContextOptions<PgSqlSuperAdminDbContext> options)
        : base(options)
    {
        // base.Database.Migrate();
    }
}