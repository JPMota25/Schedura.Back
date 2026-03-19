using Schedura.Api.Middlewares;
using Schedura.Bootstraper.DependencyInjection;
using Schedura.Infra.Configuration;

DotEnvLoader.LoadFromSolutionRoot(DotEnvLoader.ResolveEnvironmentName());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSchedura(builder.Configuration);

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseHttpsRedirection();
app.MapControllers();
app.MapGet("/", () => Results.Ok("Schedura API no ar"));

app.Run();

public partial class Program;
