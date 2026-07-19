using FluentValidation;

namespace Invento.Application.Features.Products.Commands
{
    public class CreateProductNoteCommandValidator
        : AbstractValidator<CreateProductNoteCommand>
    {
        public CreateProductNoteCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Note)
                .NotEmpty()
                .MaximumLength(5000);
        }
    }
}