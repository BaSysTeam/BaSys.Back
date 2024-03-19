using BaSys.Host.Abstractions;
using BaSys.Host.DAL;
using BaSys.SuperAdmin.Abstractions;

namespace BaSys.Host.Services;

public class WorkDbService : IWorkDbService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbInfoRecordsService _infoRecordsService;
    
    public WorkDbService(IServiceProvider serviceProvider, IDbInfoRecordsService infoRecordsService)
    {
        _serviceProvider = serviceProvider;
        _infoRecordsService = infoRecordsService;
    }

    public async Task<bool> InitDb(int dbInfoRecordId)
    {
        var dbInfo = await _infoRecordsService.GetDbInfoRecord(dbInfoRecordId);

        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();

        return false;
    }
}