using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.Data;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Services;

public class DbInfoRecordsService : IDbInfoRecordsService
{
    private readonly SuperAdminDbContext _context;
    
    public DbInfoRecordsService(SuperAdminDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DbInfoRecord>> GetDbInfoRecords()
    {  
        return await _context.DbInfoRecords
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<DbInfoRecord>> GetDbInfoRecordsByAppId(string appId)
    {
        if (string.IsNullOrEmpty(appId))
            throw new ArgumentException();

        return await _context.DbInfoRecords
            .AsNoTracking()
            .Where(x=>x.AppId.ToUpper() == appId.ToUpper())
            .ToListAsync();
    }

    public async Task<DbInfoRecord?> GetDbInfoRecord(int id)
    {
        var record = await _context.DbInfoRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        
        return record;
    }

    public async Task<DbInfoRecord> AddDbInfoRecord(DbInfoRecord dbInfoRecord)
    {
        if (dbInfoRecord == null ||
            string.IsNullOrEmpty(dbInfoRecord.AppId) ||
            string.IsNullOrEmpty(dbInfoRecord.Name) ||
            string.IsNullOrEmpty(dbInfoRecord.ConnectionString))
            throw new ArgumentException();
        
        var item = new DbInfoRecord(dbInfoRecord);
        _context.DbInfoRecords.Add(item);
        
        await _context.SaveChangesAsync();
        
        return item;
    }

    public async Task<DbInfoRecord> EditDbInfoRecord(DbInfoRecord dbInfoRecord)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecord.Id);
        if (dbItem == null)
            throw new ArgumentException($"Record not found by id: {dbInfoRecord.Id}");
        
        dbItem.Fill(dbInfoRecord);
        
        await _context.SaveChangesAsync();
        
        return dbItem;
    }

    public async Task<int> DeleteDbInfoRecord(int dbInfoRecordId)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecordId);
        if (dbItem == null)
            throw new ArgumentException($"Cannot find item by id:{dbInfoRecordId}");
        
        _context.DbInfoRecords.Remove(dbItem);
        var deletedCount =  await _context.SaveChangesAsync();
        
        return deletedCount;
    }
}