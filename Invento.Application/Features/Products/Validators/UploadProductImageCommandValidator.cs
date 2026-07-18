using FluentValidation;

namespace Invento.Application.Features.Products.Commands
{
    public class UploadProductImageCommandValidator
        : AbstractValidator<UploadProductImageCommand>
    {
        private const long MaxFileSize =
            5 * 1024 * 1024;

        public UploadProductImageCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty();

            RuleFor(x => x.Image)
                .NotNull()
                .Must(x => x.Length > 0)
                .WithMessage("Image is required.");

            RuleFor(x => x.Image.Length)
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage("Maximum file size is 5 MB.");
        }
    }
}