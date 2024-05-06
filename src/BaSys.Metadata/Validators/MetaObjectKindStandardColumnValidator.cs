using BaSys.Metadata.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Validators
{
    public sealed class MetaObjectKindStandardColumnValidator : AbstractValidator<MetaObjectKindStandardColumn>
    {
        public MetaObjectKindStandardColumnValidator()
        {
            RuleFor(x => x.Uid).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.DataTypeUid).NotEmpty();
        }
    }
}
