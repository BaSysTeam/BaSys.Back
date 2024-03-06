using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.Host.Services;
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
    }
}
