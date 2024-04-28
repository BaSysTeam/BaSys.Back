using BaSys.Metadata.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Validators
{
    public class MetadataKindSettingsValidator: AbstractValidator<MetadataKindSettings>
    {
        public MetadataKindSettingsValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x=> x.Name)
                .NotEmpty()
                .MaximumLength(30)
                .Matches("^[a-z_][a-z0-9_]*$")
                .WithMessage("Name must contain only lowercase letters, numbers, and underscores, and cannot start with a number.");
            RuleFor(x => x.NamePlural)
               .NotEmpty()
               .MaximumLength(30)
               .Matches("^[a-z_][a-z0-9_]*$")
               .WithMessage("NamePlural must contain only lowercase letters, numbers, and underscores, and cannot start with a number.");
            RuleFor(x=>x.Prefix)
                .NotEmpty()
                .MaximumLength(4)
                .Matches("^[a-z_][a-z0-9_]*$")
                .WithMessage("Prefix must contain only lowercase letters, numbers, and underscores, and cannot start with a number.");
            RuleFor(x => x.Memo).MaximumLength(300);
        }
    }
}
