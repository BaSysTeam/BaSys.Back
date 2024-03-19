using BaSys.Common.Enums;
using BaSys.Host.Infrastructure;
using BaSys.Host.Infrastructure.Interfaces;

namespace BaSys.Host.Helpers;

public class ContextHelper
{
    public static ConnectionItem? GetConnectionItem(IServiceProvider serviceProvider, DbKinds? dbKind = null)
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var userId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault()?.Value;
    
        ConnectionItem? item;
        if (!string.IsNullOrEmpty(userId))
        {
            item = serviceProvider.GetRequiredService<IDataSourceProvider>().GetCurrentConnectionItemByUser(userId);
        }
        // auth from Login form
        else if (httpContextAccessor.HttpContext?.Request.HasFormContentType == true &&
                 httpContextAccessor.HttpContext?.Request.ContentType != null && 
                 httpContextAccessor.HttpContext?.Request.Form?.TryGetValue("Input.DbName", out var dbId) == true)
        {
            item = serviceProvider.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbId);
        }
        // auth from auth endpoint
        else if (httpContextAccessor.HttpContext?.Request.Path == "/api/auth" &&
                 httpContextAccessor.HttpContext?.Request.Query.TryGetValue("dbid", out var val) == true)
        {
            var dbIdParam = val.FirstOrDefault();
            if (string.IsNullOrEmpty(dbIdParam))
                throw new ArgumentException("DbId not set in auth query!");
            
            item = serviceProvider.GetRequiredService<IDataSourceProvider>().GetConnectionItemByDbId(dbIdParam);
        }
        else
        {
            item = serviceProvider.GetRequiredService<IDataSourceProvider>().GetDefaultConnectionItem(dbKind);
        }
    
        return item;
    } 
}