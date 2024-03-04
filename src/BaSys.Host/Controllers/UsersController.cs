using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.SuperAdmin.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Controllers
{
    [Route("api/admin/v1/[controller]")]
    [ApiController]
#if !DEBUG
    [Authorize(TeamRole.Administrator)]
#endif
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {

            var result = new ResultWrapper<IEnumerable<UserDto>>();

            try
            {
                var identityUsers = await _userManager.Users.ToListAsync();
                if (identityUsers != null)
                {
                    var users = identityUsers.Select(x => new UserDto() { Id = x.Id, Email = x.Email, UserName = x.UserName });
                    result.Success(users);
                }
                else
                {
                    result.Error(-1, "Empty users list");
                }

            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get users list. Message: {ex.Message}");
            }

            return Ok(result);

        }
    }
}
