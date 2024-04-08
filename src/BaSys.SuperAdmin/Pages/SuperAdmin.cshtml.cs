using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.SuperAdmin.Pages;

[Authorize(Roles = ApplicationRole.SuperAdministrator)]
public class SuperAdmin : PageModel
{
    public void OnGet()
    {
        
    }
}