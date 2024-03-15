using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.DAL.PgSqlContext;

public class PgSqlDbContext : ApplicationDbContext
{
    public PgSqlDbContext(DbContextOptions<PgSqlDbContext> options) 
        : base(options)
    {
        base.Database.Migrate();
    }
}