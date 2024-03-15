using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.SuperAdmin.Pages;

[Authorize]
public class SuperAdmin : PageModel
{
    public void OnGet()
    {
        
    }
}