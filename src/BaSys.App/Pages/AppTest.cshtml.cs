using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BaSys.App.Pages
{
    [Authorize(Roles = ApplicationRole.User)]
    public class AppTestModel : PageModel
    {
        public string UserName { get; set; } = string.Empty;
        public void OnGet()
        {
            UserName = User.Identity.IsAuthenticated ? User.Identity.Name : "Guest";
        }
    }
}
