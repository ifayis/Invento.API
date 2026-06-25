using FluentValidation;
using Invento.Application.Features.Auth.Commands;

namespace Invento.Application.Features.Auth.Validators
{
    public class ForgotPasswordCommandValidator
        : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email address.");
        }
    }
}