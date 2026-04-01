using FluentValidation;

namespace Catalog.API.Products.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("Product id is required");
            RuleFor(p => p.Name).NotEmpty().WithMessage("Product name is required");
            RuleFor(p => p.Description).NotEmpty().WithMessage("Product description is required");
            RuleFor(p => p.Categories).NotEmpty().WithMessage("Product categories are required");
            RuleFor(p => p.Image).NotEmpty().WithMessage("Product image is required");
            RuleFor(p => p.Price).GreaterThan(0).WithMessage("Product price must be greater than 0");
        }
    }
}
