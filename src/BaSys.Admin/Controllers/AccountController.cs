using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers
{
    [Route("api/admin/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
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
