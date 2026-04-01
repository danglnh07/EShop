using BuildingBlocks.Behaviours;
using Carter;
using FluentValidation;
using Mapster;
using Marten;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
});
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DefaultConn")!);
})
.UseLightweightSessions();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies()); // Config for custom Mapster
var app = builder.Build();

// Add middleware
app.MapCarter();

app.Run();
