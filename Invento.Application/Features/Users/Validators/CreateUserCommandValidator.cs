using FluentValidation;
using Invento.Application.Features.Users.Commands;

namespace Invento.Application.Features.Users.Validators
{
    public class CreateUserCommandValidator
        : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(x => x.Role)
                .IsInEnum();
        }
    }
}