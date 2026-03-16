using BuildingBlocks.CQRS;
using Catalog.API.Errors;
using Catalog.API.Models;
using Marten;
using SharedModels.Models;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id,
                                       string Name,
                                       string Description,
                                       List<string> Categories,
                                       string Image,
                                       decimal Price) : ICommand<Result<UpdateProductResult>>;
    public record UpdateProductResult(Product? Product);

    public class UpdateProductCommandHandler(IDocumentSession session) : ICommandHandler<UpdateProductCommand, Result<UpdateProductResult>>
    {
        public async Task<Result<UpdateProductResult>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            // Get product by id
            var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
            if (product is null)
            {
                return Result<UpdateProductResult>.Failure(ProductError.ProductNotFound);
            }

            // Map from command to model
            product.Name = command.Name;
            product.Description = command.Description;
            product.Categories = command.Categories;
            product.Image = command.Image;
            product.Price = command.Price;

            // Save to database
            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);
            return Result<UpdateProductResult>.Success(new(product));
        }
    }
}
