using FluentValidation;

namespace BaSys.Admin.DTO
{
    public sealed class UserUpdateValidator: UserValidatorBase
    {
        public UserUpdateValidator(): base()
        {  
            RuleFor(x=>x.Id).NotEmpty();      
        }
    }
}
