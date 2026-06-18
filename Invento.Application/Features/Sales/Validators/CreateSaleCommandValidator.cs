using FluentValidation;
using Invento.Application.Features.Sales.Command;

namespace Invento.Application.Features.Sales.Validators
{
    public class CreateSaleCommandValidator
        : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleCommandValidator()
        {
            RuleFor(x => x.SaleDate)
                .NotEmpty();

            RuleFor(x => x.PaidAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Items)
                .NotEmpty();

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ProductId)
                        .NotEmpty();

                    item.RuleFor(x => x.Quantity)
                        .GreaterThan(0);
                });
        }
    }
}