using BaSys.Common.Enums;
using BaSys.Host.Data;
using BaSys.Host.Data.MsSqlContext;
using BaSys.Host.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.DAL;

public class ContextFactory : IContextFactory
{
    private readonly MsSqlDbContext _sqlContext;
    private readonly IDataSourceProvider _dataSourceProvider;
    private readonly IHttpContextAccessor  _httpContextAccessor;
    
    public ContextFactory(MsSqlDbContext sqlContext,
        IDataSourceProvider dataSourceProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _sqlContext = sqlContext;
        _dataSourceProvider = dataSourceProvider;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public ApplicationDbContext? GetContext()
    {
        var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault()?.Value;
        
        ApplicationDbContext? context = null;
        var currentConnection = _dataSourceProvider.GetCurrentConnectionItemByUser(userId);

        if (currentConnection?.DbKind == DbKinds.MsSql)
        {
            _sqlContext.Database.Migrate();
            context = _sqlContext;
        }

        return context;
    }
}