using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BaSys.Admin.DTO
{
    public sealed class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LockoutEnd { get; set; } = string.Empty;

        public List<UserRoleDto> Roles { get; set; } = new List<UserRoleDto>();

        public UserDto()
        {

        }

        public UserDto(IdentityUser user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            LockoutEnd = user.LockoutEnd?.ToString("yyyy-MM-dd") ?? "";
        }

        public void AddRoles(IList<string> roles)
        {
            if (roles == null)
                return;

            Roles.Clear();

            var allAppRoles = TeamRole.AllApplicationRoles();

            foreach (var appRole in allAppRoles)
            {
                var isChecked = roles.Any(x => x.Equals(appRole.Name, StringComparison.InvariantCultureIgnoreCase));
                var roleDto = new UserRoleDto { IsChecked = isChecked, Name = appRole.Name };
                Roles.Add(roleDto);
            }
        }

        public DateTimeOffset LockoutEndOffset()
        {
            if (DateTimeOffset.TryParse(LockoutEnd.ToString(), out var result))
            {
                return result;
            }
            else
            {
                return DateTimeOffset.MaxValue;
            }
        }
    }
}
