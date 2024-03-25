using System.Collections.Concurrent;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BaSys.SuperAdmin.DAL.Providers;

public class DbInfoRecordsProvider : IDbInfoRecordsProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<string, DbInfoRecord> _dict = new();
    
    public DbInfoRecordsProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Update()
    {
        _dict.Clear();
        
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SuperAdminDbContext>();

        var dbInfoRecords = await context.DbInfoRecords
            .AsNoTracking()
            .ToListAsync();
        
        foreach (var dbInfoRecord in dbInfoRecords)
        {
            _dict.TryAdd(dbInfoRecord.Name.ToUpper(), dbInfoRecord);
        }
    }
    
    public DbInfoRecord? GetDbInfoRecordByDbName(string dbName)
    {
        if (string.IsNullOrEmpty(dbName))
            return null;

        _dict.TryGetValue(dbName.ToUpper(), out var dbInfoRecord);
        return dbInfoRecord;
    }
}