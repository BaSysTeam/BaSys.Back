using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.Admin.Pages
{
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AdminModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
