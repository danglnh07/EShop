using Carter;
using Catalog.API.Models;
using Mapster;
using MediatR;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdResponse(Product Product);

    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
            {
                var query = new GetProductByIdQuery(id);
                var result = await sender.Send(query);
                if (result is null || result.Product is null)
                {
                    return Results.NotFound();
                }
                var resp = result.Adapt<GetProductByIdResponse>();
                return Results.Ok(resp);
            })
                .WithName("GetProductById")
                .WithDescription("Get a product by id")
                .WithSummary("Get a product by id")
                .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
