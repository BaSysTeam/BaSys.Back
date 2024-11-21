using BaSys.Common.Enums;
using BaSys.Metadata.Models;
using FluentValidation;

namespace BaSys.Metadata.Validators
{
    public sealed class MetaObjectCommandValidator : AbstractValidator<MetaObjectCommand>
    {
        public MetaObjectCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(x=>EmptyFieldErrorMessage(x, "Name"));
           // RuleFor(x => x.Kind).NotEmpty().WithMessage(x => EmptyFieldErrorMessage(x, "Kind"));
            RuleFor(x => x.TableUid).NotEmpty().WithMessage(x => EmptyFieldErrorMessage(x, "TableUid"));
            RuleFor(x => x.Title).NotEmpty().WithMessage(x => EmptyFieldErrorMessage(x, "Title"));

            RuleFor(x => x.Expression)
                .NotEmpty()
                .When(x => x.Kind == MetaObjectCommandKinds.Custom)
                .WithMessage(x => $"Command \"{x.ToString()}\". Expression must not be empty for Custom command.");
        }

        private string EmptyFieldErrorMessage(MetaObjectCommand command, string fieldName)
        {
            return $"Command \"{command.ToString()}\". {fieldName} must not be empty.";
        }
    }
}
