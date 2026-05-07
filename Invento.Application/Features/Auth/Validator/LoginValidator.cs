using FluentValidation;
using Invento.Application.Features.Auth.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Auth.Validator
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
