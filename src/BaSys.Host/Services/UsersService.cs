using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

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
                    result.Error(-1, $"Cannot find user by Id: {id}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get user by Id: {id}. Message: {ex.Message}");
            }

            return result;
        }

        public async Task<ResultWrapper<UserDto>> GetUserByEmailAsync(string email)
        {
            var result = new ResultWrapper<UserDto>();

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userDto = new UserDto(user);
                    userDto.AddRoles(roles);

                    result.Success(userDto);
                }
                else
                {
                    result.Error(-1, $"Cannot find user by EMail: {email}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot get user by EMail: {email}. Message: {ex.Message}");
            }

            return result;
        }

        public async Task<ResultWrapper<UserDto>> CreateUserAsync(UserDto userDto)
        {
            var result = new ResultWrapper<UserDto>();

            var savedUser = await _userManager.FindByEmailAsync(userDto.Email);

            if (savedUser != null)
            {
                result.Error(-1, $"User {userDto.UserName}/{userDto.Email} already exists.");
            }

            savedUser = await _userManager.FindByNameAsync(userDto.UserName);

            if (savedUser != null)
            {
                result.Error(-1, $"User {userDto.UserName}/{userDto.Email} already exists.");
            }

            try
            {
                var newUser = CreateUserInstance();
                newUser.Email = userDto.Email;
                newUser.UserName = userDto.UserName;

                var userCreateResult = await _userManager.CreateAsync(newUser, userDto.Password);

                if (userCreateResult.Succeeded)
                {
                    // User created. Add checked roles to user.
                    await _userManager.AddToRolesAsync(newUser, userDto.CheckedRoles);
                    
                    var getUserResult = await GetUserByEmailAsync(userDto.Email);

                    if(getUserResult.IsOK)
                    {
                        result.Success(getUserResult.Data);
                        return result;
                    }
                    else
                    {
                        return getUserResult;
                    }              
                }
                else
                {
                    // User not created.
                    var sb = new StringBuilder();
                    foreach (var error in userCreateResult.Errors)
                    {
                        sb.AppendLine(error.Description);
                    }
                    result.Error(-1, $"Cannot create user. Message: {sb}");
                }
            }
            catch(Exception ex)
            {
                result.Error(-1, $"Cannot create user. Message: {ex.Message}");
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
                    result.Error(-1, $"Cannot find user by Id: {id}");
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
                    result.Error(-1, $"Cannot find user by Id: {id}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"Cannot cannot enable user: {id}. Message: {ex.Message}");
            }


            return result;
        }

        private IdentityUser CreateUserInstance()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
