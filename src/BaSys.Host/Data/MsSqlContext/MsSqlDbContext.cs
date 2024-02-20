using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Data.MsSqlContext;

public class MsSqlDbContext : ApplicationDbContext
{
    public MsSqlDbContext(DbContextOptions<MsSqlDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }
}