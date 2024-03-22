using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.DTO;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Services;

public class DbInfoRecordsService : IDbInfoRecordsService
{
    private readonly SuperAdminDbContext _context;

    public DbInfoRecordsService(SuperAdminDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DbInfoRecordDto>> GetDbInfoRecords()
    {
        return (await _context.DbInfoRecords
                .AsNoTracking()
                .ToListAsync())
            .Select(x => new DbInfoRecordDto(x));
    }

    public async Task<DbInfoRecordDto?> GetDbInfoRecordByDbName(string dbName)
    {
        if (string.IsNullOrEmpty(dbName))
            return null;
        
        var item = await _context.DbInfoRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name.ToUpper() == dbName.ToUpper());

        if (item == null)
            return null;
        
        return new DbInfoRecordDto(item);
    }

    public async Task<IEnumerable<DbInfoRecordDto>> GetDbInfoRecordsByAppId(string appId)
    {
        if (string.IsNullOrEmpty(appId))
            throw new ArgumentException();

        return (await _context.DbInfoRecords
                .AsNoTracking()
                .Where(x => x.AppId.ToUpper() == appId.ToUpper())
                .ToListAsync())
            .Select(x => new DbInfoRecordDto(x));
    }

    public async Task<DbInfoRecordDto?> GetDbInfoRecord(int id)
    {
        var record = await _context.DbInfoRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return new DbInfoRecordDto(record);
    }

    public async Task<DbInfoRecordDto> AddDbInfoRecord(DbInfoRecordDto dbInfoRecord)
    {
        if (dbInfoRecord == null ||
            string.IsNullOrEmpty(dbInfoRecord.AppId) ||
            string.IsNullOrEmpty(dbInfoRecord.Name) ||
            string.IsNullOrEmpty(dbInfoRecord.ConnectionString))
            throw new ArgumentException();

        //var item = new DbInfoRecord(dbInfoRecord);
        var item = dbInfoRecord.ToModel();
        _context.DbInfoRecords.Add(item);

        await _context.SaveChangesAsync();

        return new DbInfoRecordDto(item);
    }

    public async Task<DbInfoRecordDto> EditDbInfoRecord(DbInfoRecordDto dbInfoRecord)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecord.Id);
        if (dbItem == null)
            throw new ArgumentException($"Record not found by id: {dbInfoRecord.Id}");

        dbItem.Fill(dbInfoRecord.ToModel());

        await _context.SaveChangesAsync();

        return new DbInfoRecordDto(dbItem);
    }

    public async Task<int> DeleteDbInfoRecord(int dbInfoRecordId)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecordId);
        if (dbItem == null)
            throw new ArgumentException($"Cannot find item by id:{dbInfoRecordId}");

        _context.DbInfoRecords.Remove(dbItem);
        var deletedCount = await _context.SaveChangesAsync();

        return deletedCount;
    }

    public async Task<DbInfoRecordDto> SwitchActivityDbInfoRecord(int dbInfoRecordId)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecordId);
        if (dbItem == null)
            throw new ArgumentException($"Cannot find item by id:{dbInfoRecordId}");

        dbItem.IsDeleted = !dbItem.IsDeleted;
        _context.Update(dbItem);
        await _context.SaveChangesAsync();

        return new DbInfoRecordDto(dbItem);
    }
}