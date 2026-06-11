using FluentValidation;
using Invento.Application.Features.Suppliers.Commands;

namespace Invento.Application.Features.Suppliers.Validators
{
    public class CreateSupplierCommandValidator
        : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.ContactPerson)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.ContactPerson));

            RuleFor(x => x.Address)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.TaxRegistrationNumber)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.TaxRegistrationNumber));
        }
    }
}