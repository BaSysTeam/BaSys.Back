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

        /// <summary>
        /// Retrieve all registered users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {

            var result = new ResultWrapper<IEnumerable<UserDto>>();

            try
            {
                var identityUsers = await _userManager.Users.ToListAsync();
                if (identityUsers != null)
                {
                    var users = identityUsers.Select(x => new UserDto(x));
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

        /// <summary>
        /// Retrieve user by Id.
        /// </summary>
        /// <param name="id">UserId</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var result = new ResultWrapper<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userDto = new UserDto(user);    
                    userDto.AddRoles(roles);

                    result.Success(id);  
                }
                else
                {
                    result.Error(-2, $"Cannot find user by Id: {id}");
                }
            }
            catch(Exception ex)
            {
                result.Error(-3, $"Cannot get user by Id: {id}. Message: {ex.Message}");
            }

            return Ok(result);
        }

        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> DisableUser(string id)
        {
            var result = new ResultWrapper<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    // Set the LockoutEnd to a date far in the future to effectively disable the account.
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                    await _userManager.UpdateAsync(user);

                    result.Success(id);
                }
                else
                {
                    result.Error(-2, $"Cannot find user by Id: {id}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-3, $"Cannot cannot disable user: {id}. Message: {ex.Message}");
            }
          

            return Ok(result);
        }

        [HttpPatch("{id}/enable")]
        public async Task<IActionResult> EnableUser(string id)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    // Set the LockoutEnd to a date far in the future to effectively disable the account.
                    user.LockoutEnd = null;
                    await _userManager.UpdateAsync(user);

                    result.Success(1);
                }
                else
                {
                    result.Error(-2, $"Cannot find user by Id: {id}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-3, $"Cannot cannot enable user: {id}. Message: {ex.Message}");
            }


            return Ok(result);
        }
    }
}
