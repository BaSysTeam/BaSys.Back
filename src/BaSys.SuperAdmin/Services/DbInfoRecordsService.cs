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
    
    public async Task<IEnumerable<DbInfoRecord>> GetDbInfoRecordsByAppId(string appId)
    {
        if (string.IsNullOrEmpty(appId))
            throw new ArgumentException();

        return await _context.DbInfoRecords
            .AsNoTracking()
            .Where(x => !x.IsDeleted &&
                        x.AppId.ToUpper() == appId.ToUpper())
            .ToListAsync();
    }

    public async Task<DbInfoRecord> AddDbInfoRecord(DbInfoRecord dbInfoRecord)
    {
        if (dbInfoRecord == null ||
            string.IsNullOrEmpty(dbInfoRecord.AppId) ||
            string.IsNullOrEmpty(dbInfoRecord.Title) ||
            string.IsNullOrEmpty(dbInfoRecord.ConnectionString))
            throw new ArgumentException();

        var item = new DbInfoRecord
        {
            AppId = dbInfoRecord.AppId,
            Title = dbInfoRecord.Title,
            DbKind = dbInfoRecord.DbKind,
            ConnectionString = dbInfoRecord.ConnectionString,
            Memo = dbInfoRecord.Memo
        };
        _context.DbInfoRecords.Add(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<DbInfoRecord> EditDbInfoRecord(DbInfoRecord dbInfoRecord)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecord.Id);
        if (dbItem == null)
            throw new ArgumentException();

        dbItem.AppId = dbInfoRecord.AppId;
        dbItem.Title = dbInfoRecord.Title;
        dbItem.DbKind = dbInfoRecord.DbKind;
        dbItem.ConnectionString = dbInfoRecord.ConnectionString;
        dbItem.Memo = dbInfoRecord.Memo;

        await _context.SaveChangesAsync();

        return dbItem;
    }

    public async Task<bool> DeleteDbInfoRecord(int dbInfoRecordId)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecordId);
        if (dbItem == null)
            throw new ArgumentException();

        _context.DbInfoRecords.Remove(dbItem);
        await _context.SaveChangesAsync();

        return true;
    }
}