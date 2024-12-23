using BaSys.Metadata.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.Validators
{
    public sealed class MetaObjectTableColumnValidator: AbstractValidator<MetaObjectTableColumn>
    {
        public MetaObjectTableColumnValidator()
        {
            RuleFor(x => x.Uid).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.DataSettings.DataTypeUid).NotEmpty();
        }
    }
}
