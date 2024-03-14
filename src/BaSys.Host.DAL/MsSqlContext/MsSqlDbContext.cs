using BaSys.Host.Data;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.DAL.MsSqlContext;

public class MsSqlDbContext : ApplicationDbContext
{
    public MsSqlDbContext(DbContextOptions<MsSqlDbContext> options)
        : base(options)
    {
        base.Database.Migrate();
    }
}