using BaSys.Metadata.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Validators
{
    public class MetaObjectKindSettingsValidator: AbstractValidator<MetaObjectKindSettings>
    {
        public MetaObjectKindSettingsValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x=> x.Name)
                .NotEmpty()
                .MaximumLength(30)
                .Matches("^[a-z_][a-z0-9_]*$")
                .WithMessage("Name must contain only lowercase letters, numbers, and underscores, and cannot start with a number.");
            RuleFor(x=>x.Prefix)
                .NotEmpty()
                .MaximumLength(4)
                .Matches("^[a-z_][a-z0-9_]*$")
                .WithMessage("Prefix must contain only lowercase letters, numbers, and underscores, and cannot start with a number.");
            RuleFor(x => x.Memo).MaximumLength(300);

            // Apply the validator for each item in the StandardColumns collection.
            RuleForEach(x => x.StandardColumns).SetValidator(new MetaObjectKindStandardColumnValidator());

            // Custom rule to ensure at least one primary key is present if StoreData is true
            RuleFor(x => x.StandardColumns)
                .Must((settings, columns) => !settings.StoreData || columns.Any(c => c.IsPrimaryKey))
                .When(x => x.IsReference)
                .WithMessage("There must be at least one primary key in StandardColumns when IsReference is true.");
        }
    }
}
