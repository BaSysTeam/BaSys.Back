using BaSys.Metadata.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Validators
{
    public class MetaObjectStorableSettingsValidator: AbstractValidator<MetaObjectStorableSettings>
    {
        public MetaObjectStorableSettingsValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(30)
                .Matches("^[a-z_][a-z0-9_]*$")
                .WithMessage("Name must contain only lowercase letters, numbers, and underscores, and cannot start with a number.");
            RuleFor(x => x.Memo).MaximumLength(300);
        }
    }
}
