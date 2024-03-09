using FluentValidation;

namespace BaSys.Admin.DTO
{
    public sealed class UserCreateValidator: AbstractValidator<UserDto>
    {
        public UserCreateValidator()
        {
            RuleFor(x=>x.UserName).NotEmpty().MaximumLength(UserDto.UserNameMaxLength);
            RuleFor(x=>x.Email).NotEmpty().MaximumLength(UserDto.EmailMaxLength).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        }
    }
}
