using Carter;
using Catalog.API.Models;
using Mapster;
using MediatR;
using SharedModels.Models;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsParamaters(int Page = 1, int Size = 10);
    public record GetProductsResponse(IEnumerable<Product> Products, Pagination Pagination);

    public class GetProductsEndpoint(ILogger<GetProductsEndpoint> logger) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async ([AsParameters] GetProductsParamaters param, ISender sender) =>
            {
                // Send command object to handler
                var result = await sender.Send(param.Adapt<GetProductsQuery>()!);

                // Create and return response
                var resp = result.Adapt<GetProductsResponse>();

                // Log some information
                logger.LogDebug($"Fetch products: page {resp!.Pagination.PageNumber}; size {resp.Pagination.PageSize}; total {resp.Pagination.TotalItems}");

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

