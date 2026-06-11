using FluentValidation;
using Invento.Application.Features.Purchases.Commands;

namespace Invento.Application.Features.Purchases.Validators
{
    public class CreatePurchaseCommandValidator
        : AbstractValidator<CreatePurchaseCommand>
    {
        public CreatePurchaseCommandValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty();

            RuleFor(x => x.Items)
                .NotEmpty();

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(x => x.ProductId)
                        .NotEmpty();

                    item.RuleFor(x => x.Quantity)
                        .GreaterThan(0);

                    item.RuleFor(x => x.UnitCost)
                        .GreaterThan(0);

                    item.RuleFor(x => x.TaxRate)
                        .GreaterThanOrEqualTo(0);
                });
        }
    }
}