using FluentValidation;
using Invento.Application.Features.Auth.Commands;

namespace Invento.Application.Features.Auth.Validators
{
    public class ChangePasswordCommandValidator
        : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.NewPassword)
                .WithMessage(
                    "Passwords do not match.");
        }
    }
}