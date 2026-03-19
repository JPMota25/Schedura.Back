using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Schedura.Api.Authorization;
using Schedura.Api.Middlewares;
using Schedura.Bootstraper.DependencyInjection;
using Schedura.Bootstraper.Seeders;
using Schedura.Infra.Configuration;

DotEnvLoader.LoadFromSolutionRoot(DotEnvLoader.ResolveEnvironmentName());

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSchedura(builder.Configuration);

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("http://localhost:5173")
		      .AllowAnyHeader()
		      .AllowAnyMethod();
	});
});

var jwtSecret = builder.Configuration["Jwt:Secret"]
	?? throw new InvalidOperationException("Jwt:Secret não configurado.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "schedura";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "schedura";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options => {
		options.TokenValidationParameters = new TokenValidationParameters {
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = jwtIssuer,
			ValidAudience = jwtAudience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
			ClockSkew = TimeSpan.Zero,
		};
	});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

var app = builder.Build();

await AdminSeeder.SeedAsync(app.Services);

app.UseExceptionMiddleware();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Ok("Schedura API no ar"));

app.Run();

public partial class Program;
