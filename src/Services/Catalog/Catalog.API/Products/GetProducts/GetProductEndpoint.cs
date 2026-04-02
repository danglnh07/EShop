using Carter;
using Catalog.API.Models;
using Mapster;
using MediatR;
using SharedModels.Models;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsParamaters(int Page = 1, int Size = 10, string? Category = null);
    public record GetProductsResponse(IEnumerable<Product> Products, Pagination Pagination);

    public class GetProductsEndpoint(ILogger<GetProductsEndpoint> logger) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async ([AsParameters] GetProductsParamaters param, ISender sender) =>
            {
                // Create query object
                var query = param.Adapt<GetProductsQuery>();
                if (query is null)
                {
                    logger.LogWarning($"Query object is null");
                    return Results.StatusCode(500);
                }

                // Send command object to handler
                var result = await sender.Send(query);

                // Create and return response
                var resp = result.Adapt<GetProductsResponse>();
                if (resp is null)
                {
                    logger.LogWarning($"Response object is null");
                    return Results.StatusCode(500);
                }

                return Results.Ok(resp);
            })
                .WithName("GetProducts")
                .WithDescription("Get products with pagination")
                .WithSummary("Get paged products")
                .Produces<GetProductsResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}

