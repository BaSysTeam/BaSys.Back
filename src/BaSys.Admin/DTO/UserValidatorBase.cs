using FluentValidation;

namespace BaSys.Admin.DTO
{
    public abstract class UserValidatorBase: AbstractValidator<UserDto>
    {
        protected UserValidatorBase()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(UserDto.UserNameMaxLength);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(UserDto.EmailMaxLength).EmailAddress();
            RuleFor(x => x.Roles).Must(ContainUserWithIsCheckedTrue)
                .WithMessage("User role is neccessary");
        }

        // This method checks if the collection meets the specific condition
        private bool ContainUserWithIsCheckedTrue(IList<UserRoleDto> roles)
        {
            return roles.Any(r => r.Name.Equals("user", StringComparison.OrdinalIgnoreCase) && r.IsChecked);
        }
    }
}
