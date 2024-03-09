using FluentValidation;

namespace BaSys.Admin.DTO
{
    public abstract class UserValidatorBase: AbstractValidator<UserDto>
    {
        protected UserValidatorBase()
        {
            RuleFor(x => x.UserName).NotEmpty().MaximumLength(UserDto.UserNameMaxLength);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(UserDto.EmailMaxLength).EmailAddress();
        }
    }
}
