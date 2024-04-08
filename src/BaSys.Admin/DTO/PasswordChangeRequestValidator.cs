using FluentValidation;

namespace BaSys.Admin.DTO
{
    public class PasswordChangeRequestValidator: AbstractValidator<PasswordChangeRequest>
    {
        public PasswordChangeRequestValidator()
        {
            RuleFor(x=>x.NewPassword).NotEmpty().MinimumLength(6);
        }
    }
}
