using FluentValidation;
using Invento.Application.Features.Auth.Commands;

namespace Invento.Application.Features.Auth.Validator
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);

            RuleFor(x => x.CompanyName)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.CompanyCode)
                .NotEmpty()
                .MaximumLength(50)
                .Matches("^[A-Za-z0-9]+$")
                .WithMessage("CompanyCode must be alphanumeric");

            RuleFor(x => x.LogoUrl)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.LogoUrl));

            RuleFor(x => x.BusinessPurpose)
                .MaximumLength(250)
                .When(x => !string.IsNullOrWhiteSpace(x.BusinessPurpose));
        }
    }
}