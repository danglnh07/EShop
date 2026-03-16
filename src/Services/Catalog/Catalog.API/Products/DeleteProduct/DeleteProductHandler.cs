using BuildingBlocks.CQRS;
using Catalog.API.Errors;
using Catalog.API.Models;
using Marten;
using SharedModels.Models;

namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : ICommand<Result>;

    public class DeleteProductCommandHandler(IDocumentSession session) : ICommandHandler<DeleteProductCommand, Result>
    {
        public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            if (await session.LoadAsync<Product>(command.Id, cancellationToken) is null)
            {
                return Result.Failure(ProductError.ProductNotFound);
            }

            session.Delete<Product>(command.Id);
            await session.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
