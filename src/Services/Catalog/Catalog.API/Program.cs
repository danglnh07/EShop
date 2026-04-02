using BuildingBlocks.Behaviours;
using Carter;
using Catalog.API.Data;
using FluentValidation;
using HealthChecks.UI.Client;
using Mapster;
using Marten;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

/* === Register services === */

// Add Carter
builder.Services.AddCarter();

// Configure MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
    config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
});

// Configure Marten
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DefaultConn")!);
})
.UseLightweightSessions();

if (builder.Environment.IsDevelopment())
{
    // Only seed data in development environment
    builder.Services.InitializeMartenWith<Initializer>();
}

// Add validators
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Configure adapter for Mapster
TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies()); // Config for custom Mapster

// Add service health check
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConn")!);

var app = builder.Build();

/* === Configure middleware === */
app.MapCarter();

app.UseExceptionHandler(config =>
{
    config.Run(async context =>
    {
        // Get exception from context
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is null)
        {
            return;
        }

        // Create problem details from exception
        var problemDetails = new ProblemDetails()
        {
            Title = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
        };

        if (exception is ValidationException validationException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
        }

        if (app.Environment.IsDevelopment())
        {
            problemDetails.Detail = exception.StackTrace;
        }

        // Get logger from context
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, exception.Message);

        // Set response status
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

app.UseHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
