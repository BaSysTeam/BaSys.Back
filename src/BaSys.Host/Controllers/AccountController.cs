using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Host.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet("CurrentUser")]
        [Authorize]
        public IActionResult CurrentUser()
        {
            var result = new ResultWrapper<UserDto>();

            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var userDto = new UserDto() { UserName = User.Identity.Name, 
                    Email = User.Identity.Name };

                result.Success(userDto);
            }
            else
            {
                result.Error(-1, "User is not logged in.");
            }

            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = new ResultWrapper<bool>();
            try
            {
                await _signInManager.SignOutAsync();

                result.Success(true, "User loged out.");
            }
            catch (Exception ex)
            {
                result.Error(-1, "Cannot logout", ex.Message);
            }

            return Ok(result);
        }
    }
}
