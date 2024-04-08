using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Identity;
using BaSys.SuperAdmin.Data.Identity;

namespace BaSys.SuperAdmin.Controllers
{
    [Route("api/sa/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<SaDbUser> _signInManager;

        public AccountController(SignInManager<SaDbUser> signInManager)
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
