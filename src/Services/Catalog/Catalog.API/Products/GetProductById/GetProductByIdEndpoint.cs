using Carter;
using Catalog.API.Models;
using Mapster;
using MediatR;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdResponse(Product Product);

    public class GetProductByIdEndpoint(ILogger<GetProductByIdEndpoint> logger) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {
                // Create query object
                var query = new GetProductByIdQuery(id);

                // Send query to handler
                var result = await sender.Send(query);
                if (result is null)
                {
                    logger.LogWarning($"Result unexpectedly is null");
                    return Results.StatusCode(500);
                }

                // Check if result.Product is null
                if (result.Product is null)
                {
                    return Results.NotFound();
                }

                // Create response
                var resp = result.Adapt<GetProductByIdResponse>();
                return Results.Ok(resp);
            })
                .WithName("GetProductById")
                .WithDescription("Get a product by id")
                .WithSummary("Get a product by id")
                .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
