﻿using BaSys.Common.Infrastructure;
using FluentValidation;

namespace BaSys.Admin.DTO
{
    public sealed class UserCreateValidator: UserValidatorBase
    {
        public UserCreateValidator(): base()
        {
            RuleFor(x => x.Password).NotEmpty().MinimumLength(GlobalConstants.PasswordMinLength);
        }
    }
}
