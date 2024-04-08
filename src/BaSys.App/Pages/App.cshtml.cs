using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.App.Pages
{
    [Authorize(Roles = ApplicationRole.User)]
    public class AppModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
