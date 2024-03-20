using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaSys.Host.Controllers
{
    /// <summary>
    /// Manages user account operations such as retrieving current user information and logging out.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        /// <summary>
        /// Gets the current authenticated user's information.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing either the current user's details or an error message if not logged in.</returns>
        /// <remarks>
        /// Requires the user to be authenticated.
        /// </remarks>
        [HttpGet("CurrentUser")]
        [Authorize]
        public IActionResult CurrentUser()
        {
            var result = new ResultWrapper<UserDto>();

            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var userDto = new UserDto() { UserName = User?.Identity?.Name ?? "Unknown", 
                    Email = GetCurrentUserEmail() };

                result.Success(userDto);
            }
            else
            {
                result.Error(-1, "User is not logged in.");
            }

            return Ok(result);
        }

        // <summary>
        /// Logs out the currently authenticated user.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> indicating the success or failure of the logout operation.</returns>
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

        private string GetCurrentUserEmail()
        {
            // Attempt to retrieve the email claim
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim != null)
            {
                string userEmail = emailClaim.Value;
                return userEmail;
            }
            else
            {
                return "Unknown";
            }
        }
    }
}
