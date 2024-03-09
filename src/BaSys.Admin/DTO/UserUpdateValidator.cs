using FluentValidation;

namespace BaSys.Admin.DTO
{
    public sealed class UserUpdateValidator: AbstractValidator<UserDto>
    {
        public UserUpdateValidator()
        {
            RuleFor(x=>x.Id).NotEmpty();
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(UserDto.UserNameMaxLength);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(UserDto.EmailMaxLength).EmailAddress();
        }
    }
}
