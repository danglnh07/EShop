using Carter;
using Mapster;
using Marten;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DefaultConn")!);
})
.UseLightweightSessions();
TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies()); // Config for custom Mapster
var app = builder.Build();

// Add middleware
app.MapCarter();

app.Run();
