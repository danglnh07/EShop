using BuildingBlocks.CQRS;
using Catalog.API.Models;
using Marten;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name,
                                       string Description,
                                       List<string> Categories,
                                       string Image,
                                       decimal Price) : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);

    internal class CreateProductCommandHandler(IDocumentSession session) : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // Create entity
            var product = new Product()
            {
                Name = command.Name,
                Description = command.Description,
                Categories = command.Categories,
                Image = command.Image,
                Price = command.Price
            };

            // Store product into database as document using Marten
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);

            // Return result with product ID
            return new(product.Id);
        }
    }
}
