using FluentValidation;

namespace Catalog.API.Products.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Product name is required");
            RuleFor(p => p.Description).NotEmpty().WithMessage("Product description is required");
            RuleFor(p => p.Categories).NotEmpty().WithMessage("Product categories are required");
            RuleFor(p => p.Image).NotEmpty().WithMessage("Product image is required");
            RuleFor(p => p.Price).GreaterThan(0).WithMessage("Product price must be greater than 0");
        }
    }
}
