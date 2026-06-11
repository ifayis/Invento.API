using FluentValidation;
using Invento.Application.Features.Suppliers.Commands;

namespace Invento.Application.Features.Suppliers.Validators
{
    public class UpdateSupplierCommandValidator
        : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}