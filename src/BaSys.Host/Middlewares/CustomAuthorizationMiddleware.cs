using System.Security.Claims;
using BaSys.Common.Infrastructure;
using BaSys.Host.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                if (result.Succeeded)
                {
                    var userManager = serviceScopeInner.ServiceProvider.GetRequiredService<UserManager<WorkDbUser>>();
                    var currentUser = await userManager.Users.FirstAsync(x => x.Email!.ToUpper() == authStrArr[0].ToUpper());
                    
                    var identity = new ClaimsIdentity(new List<Claim>
                    {
                        new ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", currentUser.Id, ClaimValueTypes.String),
                        new ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", currentUser.UserName ?? string.Empty, ClaimValueTypes.String),
                        new ("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", currentUser.Email ?? string.Empty, ClaimValueTypes.String),
                        new ("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", ApplicationRole.Designer, ClaimValueTypes.String),
                        new ("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", ApplicationRole.Administrator, ClaimValueTypes.String),
                        new ("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", ApplicationRole.User, ClaimValueTypes.String)
                    }, "VueAuth");
                    
                    context.User = new ClaimsPrincipal(identity);
                }
            }
        }
        
        await _authorizationMiddleware.Invoke(context);
    }
}