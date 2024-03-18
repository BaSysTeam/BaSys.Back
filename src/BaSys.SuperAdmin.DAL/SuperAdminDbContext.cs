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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AppRecord
        // Id
        modelBuilder.Entity<AppRecord>()
            .Property(x => x.Id)
            .HasMaxLength(50);

        // Title
        modelBuilder.Entity<AppRecord>()
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        // Memo
        modelBuilder.Entity<AppRecord>()
            .Property(x => x.Memo)
            .HasMaxLength(300);
        
        // DbInfoRecord
        // AppId
        modelBuilder.Entity<DbInfoRecord>()
            .HasOne(x => x.App)
            .WithMany(x => x.DbInfoRecords)
            .HasForeignKey(x => x.AppId);
        modelBuilder.Entity<DbInfoRecord>()
            .Property(x => x.AppId)
            .IsRequired()
            .HasMaxLength(50);
        
        // Name
        modelBuilder.Entity<DbInfoRecord>()
            .HasIndex(x => x.Name)
            .IsUnique();
        modelBuilder.Entity<DbInfoRecord>()
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        // Title
        modelBuilder.Entity<DbInfoRecord>()
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        // ConnectionString
        modelBuilder.Entity<DbInfoRecord>()
            .Property(x => x.ConnectionString)
            .IsRequired();

        // Memo
        modelBuilder.Entity<DbInfoRecord>()
            .Property(x => x.Memo)
            .HasMaxLength(300);
    }
}