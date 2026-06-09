using FluentValidation;
using Invento.Application.Features.Products.Commands;

namespace Invento.Application.Features.Products.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.SKU)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SellingPrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CategoryId)
                .NotEmpty();
        }
    }
}
