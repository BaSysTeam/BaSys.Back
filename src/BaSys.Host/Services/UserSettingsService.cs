using BaSys.Common.Infrastructure;
using BaSys.DAL.Models.App;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Abstractions;

namespace BaSys.Host.Services;

public class UserSettingsService : IUserSettingsService
{
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IBaSysConnectionFactory _baSysConnectionFactory;
    
    public UserSettingsService(IDbInfoRecordsProvider dbInfoRecordsProvider,
        IBaSysConnectionFactory baSysConnectionFactory)
    {
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _baSysConnectionFactory = baSysConnectionFactory;
    }
    
    public async Task<ResultWrapper<UserSettings?>> GetUserSettings(string userId)
    {
        // var result = new ResultWrapper<int>();
        // var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);
        
        return null;
    }
    
    public async Task<ResultWrapper<bool>> UpdateUserSettings(UserSettings userSettings)
    {
        return null;
    }
}