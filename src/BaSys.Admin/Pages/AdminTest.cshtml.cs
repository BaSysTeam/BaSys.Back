using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Admin.Pages
{
    [Authorize(Roles = ApplicationRole.Administrator)]
    public class AdminTestModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        public IList<IdentityUser> Users { get; set; } = new List<IdentityUser>();

        public AdminTestModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();
        }
    }
}
