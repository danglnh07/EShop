using Carter;
using Mapster;
using MediatR;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductRequest(string Name, string Description, List<string> Categories, string Image, decimal Price);
    public record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint(ILogger<CreateProductEndpoint> logger) : ICarterModule
    {
        private readonly ILogger<CreateProductEndpoint> _logger = logger;

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest req, ISender sender) =>
            {
                // Map from request to command object
                var command = req.Adapt<CreateProductCommand>();
                if (command is null)
                {
                    _logger.LogWarning($"Command object is null");
                    return Results.StatusCode(500);
                }

                // Send command object to handler
                var result = await sender.Send(command);

                // Create and return response
                var resp = result.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{result.Id}", resp);
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
