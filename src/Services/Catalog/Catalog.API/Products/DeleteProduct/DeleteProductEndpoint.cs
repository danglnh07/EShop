using Carter;
using Catalog.API.Errors;
using MediatR;

namespace Catalog.API.Products.DeleteProduct
{
    public class DeleteProductEndpoint(ILogger<DeleteProductEndpoint> logger) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id}", async (Guid id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteProductCommand(id));
                if (result.IsFailure)
                {
                    if (result.Error.Code == ProductError.ProductNotFound.Code)
                    {
                        return Results.NotFound();
                    }

                    logger.LogError($"{result.Error.Code}: {result.Error.Message}");
                    return Results.StatusCode(500);
                }

                return Results.NoContent();
            })
                .WithName("DeleteProduct")
                .WithDescription("Delete a product by Id")
                .WithSummary("Delete a product by Id")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
