using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL;
using BaSys.SuperAdmin.DAL.Abstractions;
using BaSys.SuperAdmin.DTO;
using Microsoft.EntityFrameworkCore;

namespace BaSys.SuperAdmin.Services;

public class DbInfoRecordsService : IDbInfoRecordsService
{
    private readonly SuperAdminDbContext _context;
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly ICheckDbExistsService _checkDbExistsService;

    public DbInfoRecordsService(SuperAdminDbContext context,
        IDbInfoRecordsProvider dbInfoRecordsProvider,
        ICheckDbExistsService checkDbExistsService)
    {
        _context = context;
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _checkDbExistsService = checkDbExistsService;
    }

    public async Task<IEnumerable<DbInfoRecordDto>> GetDbInfoRecords()
    {
        var dbInfoRecords = (await _context.DbInfoRecords
                .AsNoTracking()
                .ToListAsync())
            .Select(x => new DbInfoRecordDto(x))
            .ToList();

        return dbInfoRecords;
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
        await _dbInfoRecordsProvider.Update();

        return new DbInfoRecordDto(item);
    }

    public async Task<DbInfoRecordDto> EditDbInfoRecord(DbInfoRecordDto dbInfoRecord)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecord.Id);
        if (dbItem == null)
            throw new ArgumentException($"Record not found by id: {dbInfoRecord.Id}");

        dbItem.Fill(dbInfoRecord.ToModel());

        await _context.SaveChangesAsync();
        await _dbInfoRecordsProvider.Update();

        return new DbInfoRecordDto(dbItem);
    }

    public async Task<int> DeleteDbInfoRecord(int dbInfoRecordId)
    {
        var dbItem = await _context.DbInfoRecords.FirstOrDefaultAsync(x => x.Id == dbInfoRecordId);
        if (dbItem == null)
            throw new ArgumentException($"Cannot find item by id:{dbInfoRecordId}");

        _context.DbInfoRecords.Remove(dbItem);
        var deletedCount = await _context.SaveChangesAsync();
        await _dbInfoRecordsProvider.Update();

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
        await _dbInfoRecordsProvider.Update();

        return new DbInfoRecordDto(dbItem);
    }

    public async Task<IEnumerable<ExistsDbResponseDto>> CheckDbExists(IEnumerable<int> dbInfoRecordIds)
    {
        var dbInfoRecordsDict = await _context.DbInfoRecords
            .AsNoTracking()
            .Where(x => dbInfoRecordIds.Contains(x.Id))
            .ToDictionaryAsync(k => k.Id, v => v);
            
        var list = new List<ExistsDbResponseDto>();
        foreach (var dbInfoRecordId in dbInfoRecordIds)
        {
            if (!dbInfoRecordsDict.TryGetValue(dbInfoRecordId, out var item))
                continue;

            var isExists = await _checkDbExistsService.IsExists(item);
            list.Add(new ExistsDbResponseDto
            {
                DbInfoRecordId = dbInfoRecordId,
                IsExists = isExists
            });
        }

        return list;
    }
}