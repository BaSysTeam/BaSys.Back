using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Data.PgSqlContext;

public class PgSqlDbContext : ApplicationDbContext
{
    public PgSqlDbContext(DbContextOptions<PgSqlDbContext> options) 
        : base(options)
    {
        Database.Migrate();
    }
}