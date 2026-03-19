using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schedura.Application.Contracts.Persons;
using Schedura.Application.Contracts.Users;
using Schedura.Application.Persons;
using Schedura.Application.Users;
using Schedura.Domain.Interfaces.Services.Persons;
using Schedura.Domain.Interfaces.Services.Users;
using Schedura.Infra.DependencyInjection;
using Schedura.Services.Persons;
using Schedura.Services.Users;

namespace Schedura.Bootstraper.DependencyInjection;

public static class ServiceCollectionExtensions {
	public static IServiceCollection AddSchedura(this IServiceCollection services, IConfiguration configuration) {
		services.AddAutoMapper(typeof(UserApplicationMappingProfile).Assembly);
		services.AddScoped<IPersonApplication, PersonApplication>();
		services.AddScoped<IUserApplication, UserApplication>();
		services.AddScoped<IPersonService, PersonService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IValidator<CreatePersonInput>, CreatePersonInputValidator>();
		services.AddScoped<IValidator<CreateUserInput>, CreateUserInputValidator>();
		services.AddScoped<IValidator<UpdateUserInput>, UpdateUserInputValidator>();
		services.AddInfrastructure(configuration);

		return services;
	}
}
