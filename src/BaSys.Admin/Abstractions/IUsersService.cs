using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;

namespace BaSys.Admin.Abstractions;

public interface IUsersService
{
    Task<ResultWrapper<IEnumerable<UserDto>>> GetAllUsers();
    Task<ResultWrapper<UserDto>> GetUserAsync(string id);
    Task<ResultWrapper<UserDto>> GetUserByEmailAsync(string email);
    Task<ResultWrapper<UserDto>> CreateUserAsync(UserDto userDto, string? dbName = null);
    Task<ResultWrapper<UserDto>> UpdateUser(UserDto userDto);
    Task<ResultWrapper<string>> DisableUserAsync(string id);
    Task<ResultWrapper<string>> EnableUserAsync(string id);
    Task<ResultWrapper<int>> DeleteUserAsync(string id);
    Task<ResultWrapper<int>> ChangePasswordAsync(string id, PasswordChangeRequest passwordChangeRequest);
}