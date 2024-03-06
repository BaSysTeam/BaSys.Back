using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Host.Services
{
    public sealed class UsersService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResultWrapper<IEnumerable<UserDto>>> GetAllUsers()
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

            return result;

        }

        public async Task<ResultWrapper<UserDto>> GetUserAsync(string id)
        {
            var result = new ResultWrapper<UserDto>();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userDto = new UserDto(user);
                    userDto.AddRoles(roles);

                    result.Success(userDto);
                }
                else
                {
                    result.Error(-2, $"Cannot find user by Id: {id}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-3, $"Cannot get user by Id: {id}. Message: {ex.Message}");
            }

            return result;
        }

        public async Task<ResultWrapper<string>> DisableUserAsync(string id)
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


            return result;
        }

        public async Task<ResultWrapper<string>> EnableUserAsync(string id)
        {
            var result = new ResultWrapper<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    // Set the LockoutEnd to a date far in the future to effectively disable the account.
                    user.LockoutEnd = null;
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
                result.Error(-3, $"Cannot cannot enable user: {id}. Message: {ex.Message}");
            }


            return result;
        }
    }
}
