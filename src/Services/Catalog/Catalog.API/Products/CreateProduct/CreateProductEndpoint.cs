using Carter;
using Mapster;
using MediatR;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductRequest(string Name, string Description, List<string> Categories, string Image, decimal Price);
    public record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint(ILogger<CreateProductEndpoint> logger) : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest req, ISender sender) =>
            {
                // Map from request to command object
                var command = req.Adapt<CreateProductCommand>();
                if (command is null)
                {
                    // Command object is null when the request itself is null -> client error
                    logger.LogWarning($"Command object is null");
                    return Results.BadRequest();
                }

                // Send command object to handler
                var result = await sender.Send(command);
                if (result is null)
                {
                    // We never return null in the handler (even for a 204 API),
                    // so this is clearly an unexpected error -> server side error
                    logger.LogError($"Result unexpectedly is null");
                    return Results.StatusCode(500);
                }

                // Check if result is success
                if (result.IsFailure)
                {
                    logger.LogError($"{result.Error.Code}: {result.Error.Message}");
                    return Results.StatusCode(500);
                }

                // Create and return response
                var resp = result.Value.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{resp!.Id}", resp);
            })
                .WithName("CreateProduct")
                .WithDescription("Create a new product")
                .WithSummary("Create a new product")
                .Produces<CreateProductResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
