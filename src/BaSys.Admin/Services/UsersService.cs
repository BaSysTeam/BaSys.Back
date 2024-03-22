using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.Translation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using BaSys.Admin.Abstractions;
using BaSys.Host.Identity;
using BaSys.Host.Identity.Models;

namespace BaSys.Admin.Services
{
    public sealed class UsersService : IUsersService
    {
        private readonly UserManager<WorkDbUser> _userManager;

        public UsersService(UserManager<WorkDbUser> userManager)
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
                    result.Success(users, DictMain.UsersListRefreshed);
                }
                else
                {
                    result.Error(-1, DictMain.EmptyUsersList);
                }

            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotGetUsersList, ex.Message);
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
                    result.Error(-1, DictMain.CannotFindUser, id);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotFindUser,$"Id: {id}, {ex.Message}");
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
                    result.Error(-1, $"{DictMain.CannotFindUser}: {email}");
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotFindUser}: {email}.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<UserDto>> CreateUserAsync(UserDto userDto)
        {
            var result = new ResultWrapper<UserDto>();

            var validator = new UserCreateValidator();
            var validationResult = validator.Validate(userDto);

            if (!validationResult.IsValid)
            {
                result.Error(-1, $"{DictMain.CannotCreateUser}. {validationResult}");
                return result;
            }

            var savedUser = await _userManager.FindByEmailAsync(userDto.Email);

            if (savedUser != null)
            {
                result.Error(-1, $"{DictMain.UserAlreadyExists}: {userDto.UserName}/{userDto.Email}");
            }

            savedUser = await _userManager.FindByNameAsync(userDto.UserName);

            if (savedUser != null)
            {
                result.Error(-1, $"{DictMain.UserAlreadyExists}: {userDto.UserName}/{userDto.Email}");
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

                    if (getUserResult.IsOK)
                    {
                        result.Success(getUserResult.Data, DictMain.UserCreated);
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
                    result.Error(-1, DictMain.CannotCreateUser, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotCreateUser, ex.Message);
            }


            return result;
        }

        public async Task<ResultWrapper<UserDto>> UpdateUser(UserDto userDto)
        {
            var result = new ResultWrapper<UserDto>();

            var validator = new UserUpdateValidator();
            var validationResult = validator.Validate(userDto);

            if (!validationResult.IsValid)
            {
                result.Error(-1, $"{DictMain.CannotUpdateUser}. {validationResult}");
                return result;
            }

            var savedUser = await _userManager.FindByIdAsync(userDto.Id);

            if (savedUser == null)
            {
                result.Error(-1, DictMain.CannotUpdateUser, userDto.Id);
                return result;
            }

            savedUser.Email = userDto.Email;
            savedUser.UserName = userDto.UserName;
            savedUser.NormalizedUserName = savedUser.UserName.ToUpper();
            savedUser.NormalizedEmail = savedUser.Email.ToUpper();

            try
            {
                var checkResult = await AreActiveUsersInRole(userDto.Id, ApplicationRole.Administrator);
                if (!checkResult && !userDto.CheckedRoles.Contains(ApplicationRole.Administrator))
                {
                    result.Error(-1, DictMain.CannotUpdateUser, DictMain.OnlyActiveAdministrator);
                }
                else
                {
                    await _userManager.UpdateAsync(savedUser);

                    var userRoles = await _userManager.GetRolesAsync(savedUser);
                    await _userManager.RemoveFromRolesAsync(savedUser, userRoles);
                    await _userManager.AddToRolesAsync(savedUser, userDto.CheckedRoles);

                    result = await GetUserAsync(userDto.Id);
                    result.Message = DictMain.UserUpdated;
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotUpdateUser, ex.Message);
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
                    var checkResult = await AreActiveUsersInRole(id, ApplicationRole.Administrator);
                    if (!checkResult)
                    {
                        result.Error(-1, DictMain.CannotDisableUser, DictMain.OnlyActiveAdministrator);
                    }
                    else
                    {
                        // Set the LockoutEnd to a date far in the future to effectively disable the account.
                        user.LockoutEnd = DateTimeOffset.MaxValue;
                        await _userManager.UpdateAsync(user);

                        result.Success(id, DictMain.UserDisabled);
                    }
                }
                else
                {
                    result.Error(-1, DictMain.CannotFindUser, id);
                }
            }
            catch (Exception ex)
            {
                result.Error(-3, DictMain.CannotDisableUser, ex.Message);
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

                    result.Success(id, DictMain.UserEnabled);
                }
                else
                {
                    result.Error(-1, DictMain.CannotFindUser, id);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotEnableUser, ex.Message);
            }


            return result;
        }

        public async Task<ResultWrapper<int>> DeleteUserAsync(string id)
        {
            var result = new ResultWrapper<int>();

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var checkResult = await AreActiveUsersInRole(id, ApplicationRole.Administrator);
                    if (!checkResult)
                    {
                        result.Error(-1, DictMain.CannotDeleteUser, DictMain.OnlyActiveAdministrator);
                    }
                    else
                    {
                        var deleteResult = await _userManager.DeleteAsync(user);
                        if (deleteResult.Succeeded)
                        {
                            result.Success(1, DictMain.UserDeleted);
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            foreach (var error in deleteResult.Errors)
                            {
                                sb.AppendLine(error.Description);
                            }
                            result.Error(-1, DictMain.CannotDeleteUser, sb.ToString());
                        }
                    }
                }
                else
                {
                    result.Error(-1, DictMain.CannotFindUser, id);
                }
            }
            catch (Exception ex)
            {
                result.Error(-1, $"{DictMain.CannotDeleteUser}: {id}.", ex.Message);
            }

            return result;
        }

        public async Task<ResultWrapper<int>> ChangePasswordAsync(string id, PasswordChangeRequest passwordChangeRequest)
        {
            var result = new ResultWrapper<int>();

            var validator = new PasswordChangeRequestValidator();
            var validationResult = validator.Validate(passwordChangeRequest);

            if (!validationResult.IsValid)
            {
                result.Error(-1, $"{DictMain.WrongPasswordFormat}. {validationResult}");
                return result;
            }

            var password = passwordChangeRequest.NewPassword;

            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    result.Error(-1, DictMain.CannotFindUser, id);
                    return result;
                }

                var removeResult = await _userManager.RemovePasswordAsync(user);
                var setResult = await _userManager.AddPasswordAsync(user, password);

                if (setResult.Succeeded)
                {
                    result.Success(1);
                }
                else
                {
                    var message = BuildMessageFromIdentityResult(setResult);
                    result.Error(-1, DictMain.CannotChangePassword, message);
                }

            }
            catch (Exception ex)
            {
                result.Error(-1, DictMain.CannotChangePassword, ex.Message);
            }


            return result;
        }

        private WorkDbUser CreateUserInstance()
        {
            try
            {
                return Activator.CreateInstance<WorkDbUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(WorkDbUser)}'. " +
                    $"Ensure that '{nameof(WorkDbUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private string BuildMessageFromIdentityResult(IdentityResult result)
        {
            var sb = new StringBuilder();
            foreach (var error in result.Errors)
            {
                sb.AppendLine(error.Description);
            }

            return sb.ToString();
        }

        private async Task<bool> AreActiveUsersInRole(string exceptUserId, string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            users = users.Where(x => x.Id != exceptUserId && x.LockoutEnd == null).ToList();
            if (users.Any())
                return true;
            else
                return false;
        }
    }
}
