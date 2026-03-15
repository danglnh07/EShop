using Carter;
using Catalog.API.Models;
using Mapster;
using MediatR;
using SharedModels.Models;

namespace Catalog.API.Products.GetProductsByCategory
{
    public record GetProductsByCategoriesParam(string Category, int Page = 1, int Size = 10);
    public record GetProductsByCategoriesResponse(IEnumerable<Product> Products, Pagination Pagination);

    public class GetProductsByCategoriesEndpoint(ILogger<GetProductsByCategoriesEndpoint> logger) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category", async ([AsParameters] GetProductsByCategoriesParam param, ISender sender) =>
            {
                // Map from request to query object
                var query = param.Adapt<GetProductsByCategoryQuery>();
                if (query is null)
                {
                    logger.LogWarning($"Query object is null");
                    return Results.StatusCode(500);
                }

                // Send query object to handler
                var result = await sender.Send(query);

                // Create and return response
                var resp = result.Adapt<GetProductsByCategoriesResponse>();
                return Results.Ok(resp);
            })
                .WithName("GetProductsByCategory")
                .WithDescription("Get products by category")
                .WithSummary("Get products by category")
                .Produces<GetProductsByCategoriesResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
