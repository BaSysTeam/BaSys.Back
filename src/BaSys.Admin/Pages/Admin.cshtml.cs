using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.Admin.Pages
{
    [Authorize(Roles = TeamRole.Administrator)]
    public class AdminModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
