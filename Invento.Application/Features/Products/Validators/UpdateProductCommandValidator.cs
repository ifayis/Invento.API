using FluentValidation;
using Invento.Application.Features.Products.Commands;

namespace Invento.Application.Features.Products.Validators
{
    public class UpdateProductCommandValidator
        : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.SKU)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CategoryId)
                .NotEmpty();

            RuleFor(x => x.SellingPrice)
                .GreaterThan(0);

            RuleFor(x => x.LowStockThreshold)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CriticalStockThreshold)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x)
                .Must(x =>
                    x.CriticalStockThreshold
                    <= x.LowStockThreshold)
                .WithMessage(
                    "Critical stock threshold cannot be greater than low stock threshold.");
        }
    }
}