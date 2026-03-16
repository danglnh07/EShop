using Carter;
using Catalog.API.Errors;
using Catalog.API.Models;
using Mapster;
using MediatR;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductRequest(Guid Id,
                                       string Name,
                                       string Description,
                                       List<string> Categories,
                                       string Image,
                                       decimal Price);
    public record UpdateProductResponse(Product Product);

    public class UpdateProductEndpoint(ILogger<UpdateProductEndpoint> logger) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products/{id}", async (UpdateProductRequest req, Guid id, ISender sender) =>
            {
                // Map from request to command object
                var command = req.Adapt<UpdateProductCommand>();
                if (command is null)
                {
                    logger.LogWarning($"Command object is null");
                    return Results.StatusCode(500);
                }

                // Send command object to handler
                var result = await sender.Send(command);
                if (result is null)
                {
                    logger.LogWarning($"Result unexpectedly is null");
                    return Results.StatusCode(500);
                }

                if (result.IsFailure)
                {
                    if (result.Error.Code == ProductError.ProductNotFound.Code)
                    {
                        return Results.NotFound(result.Error.Message);
                    }

                    logger.LogError($"{result.Error.Code}: {result.Error.Message}");
                    return Results.StatusCode(500);
                }

                // Create and return response
                var resp = result.Value.Adapt<UpdateProductResponse>();
                return Results.Ok(resp);
            })
                .WithName("UpdateProduct")
                .WithDescription("Update a product")
                .WithSummary("Update a product")
                .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
