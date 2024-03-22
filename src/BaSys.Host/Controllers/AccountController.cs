using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BaSys.Host.Identity;
using BaSys.Host.Identity.Models;
using BaSys.Host.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Controllers
{
    /// <summary>
    /// Manages user account operations such as retrieving current user information and logging out.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<WorkDbUser> _signInManager;
        private readonly UserManager<WorkDbUser> _userManager;
        private readonly IDataSourceProvider _dataSourceProvider;

        public AccountController(SignInManager<WorkDbUser> signInManager,
            UserManager<WorkDbUser> userManager,
            IDataSourceProvider dataSourceProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dataSourceProvider = dataSourceProvider;
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
                var email = GetCurrentUserEmail();
                await _signInManager.SignOutAsync();
                
                var currentUser = await _userManager.Users.FirstAsync(x => x.Email.ToUpper() == email.ToUpper());
                _dataSourceProvider.RemoveConnection(currentUser.Id);

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
