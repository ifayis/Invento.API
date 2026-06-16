using FluentValidation;
using Invento.Application.Features.Balance.Commands;

namespace Invento.Application.Features.Balance.Validators
{
    public class CreateManualExpenseCommandValidator
        : AbstractValidator<CreateManualExpenseCommand>
    {
        public CreateManualExpenseCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
}