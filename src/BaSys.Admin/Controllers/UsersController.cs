using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers
{
    /// <summary>
    /// This controller allow to manipulate with Identity users. 
    /// Implemented CRUD operations and extra operations: Disable, Enable user and ChangePassword.
    /// To Disable, Enable user LockoutEnd property used.
    /// </summary>
    [Route("api/admin/v1/[controller]")]
    [ApiController]
#if !DEBUG
    [Authorize(ApplicationRole.Administrator)]
#endif
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UsersService _usersService;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _usersService = new UsersService(_userManager);
        }

        /// <summary>
        /// Retrieve all registered users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _usersService.GetAllUsers();

            return Ok(result);
        }

        /// <summary>
        /// Retrieve user by Id.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = await _usersService.GetUserAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Creates new user and set rights to user.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            var result = await _usersService.CreateUserAsync(userDto);

            return Ok(result);
        }

        /// <summary>
        /// Update user. 
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            var result = await _usersService.UpdateUser(userDto);

            return Ok(result);
        }

        /// <summary>
        /// Disable user by setting LockoutEnd maximum value.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> DisableUser(string id)
        {
            var result = await _usersService.DisableUserAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Enable user by setting LockoutEnd value null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/enable")]
        public async Task<IActionResult> EnableUser(string id)
        {
            var result = await _usersService.EnableUserAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Change password for user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/password")]
        public async Task<IActionResult> EnableUser(string id, [FromBody] PasswordChangeRequest passwordChangeRequest)
        {
            var result = await _usersService.ChangePasswordAsync(id, passwordChangeRequest);

            return Ok(result);
        }

        /// <summary>
        /// Delete user by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _usersService.DeleteUserAsync(id);

            return Ok(result);
        }
    }
}
