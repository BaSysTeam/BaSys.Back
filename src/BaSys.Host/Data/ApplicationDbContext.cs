using BaSys.Host.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Foo> FooTable { get; set; }
    
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }
}