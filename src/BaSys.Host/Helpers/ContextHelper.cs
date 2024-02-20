using BaSys.Host.Infrastructure;
using BaSys.Host.Providers;
using DbKinds = BaSys.Host.Infrastructure.DbKinds;

namespace BaSys.Host.Helpers;

public class ContextHelper
{
    public static ConnectionItem GetConnectionItem(IServiceProvider serviceProvider, DbKinds? dbKind = null)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault()?.Value;

        ConnectionItem? item = null;
        if (!string.IsNullOrEmpty(userId))
        {
            item = serviceProvider.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(userId);
        }
        else if (httpContextAccessor.HttpContext?.Request.ContentType != null && 
                 httpContextAccessor.HttpContext?.Request.Form?.TryGetValue("Input.DbName", out var dbId) == true)
        {
            item = serviceProvider.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbId);
        }
        else
        {
            item = serviceProvider.GetRequiredService<IDataSourceProvider>().GetDefaultConnectionItem(dbKind);
        }

        return item;
    } 
}