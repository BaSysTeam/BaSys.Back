using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace BaSys.PublicAPI.Infrastructure;

public class PublicApiLogHelper
{
    public static string GetMessage(HttpContext httpContext, Dictionary<string, object>? parameters = null)
    {
        var userUid = httpContext.User.Claims.FirstOrDefault(x => x.Type == "jti")?.Value;
        var userName = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var dbName = httpContext.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;

        var path = httpContext.Request.Path;
        var method = httpContext.Request.Method;

        var paramsSb = new StringBuilder();
        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                paramsSb.Append($"{parameter.Key}: {parameter.Value.ToString()}; ");
            }
        }

        var sb = new StringBuilder();
        sb.Append($"UserUid: {userUid}; ");
        sb.Append($"UserName: {userName}; ");
        sb.Append($"DbName: {dbName}; ");
        sb.Append($"Path: {path}; ");
        sb.Append($"Method: {method}; ");
        if (parameters != null)
        {
            sb.Append($"Parameters: ");
            sb.Append(paramsSb.ToString());
        }

        return sb.ToString();
    }
}