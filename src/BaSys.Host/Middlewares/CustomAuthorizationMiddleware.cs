using BaSys.Host.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BaSys.Host.Middlewares;

public class CustomAuthorizationMiddleware
{
    private readonly AuthorizationMiddleware _authorizationMiddleware;
    private readonly IServiceProvider _serviceProvider;
    
    public CustomAuthorizationMiddleware(RequestDelegate next,
        IAuthorizationPolicyProvider policyProvider,
        IServiceProvider serviceProvider)
    {
        _authorizationMiddleware = new AuthorizationMiddleware(next, policyProvider);
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        var authHeader = context.Request.Headers["CustomAuth"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader))
        {
            var authStrArr = authHeader.Split('|');
            if (authStrArr.Length == 2)
            {
                using var serviceScopeInner = _serviceProvider.CreateScope();
                var signInManager = serviceScopeInner.ServiceProvider.GetRequiredService<SignInManager<WorkDbUser>>();
                
                var result = await signInManager.PasswordSignInAsync(authStrArr[0], authStrArr[1], false, false);
            }
        }
        
        await _authorizationMiddleware.Invoke(context);
    }
    
    public string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
}