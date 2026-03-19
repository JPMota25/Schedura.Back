using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Schedura.Application.Contracts.Auth;
using Schedura.Application.Contracts.Permissions;
using Schedura.Application.Contracts.Persons;
using Schedura.Application.Contracts.UserGroups;
using Schedura.Application.Contracts.Users;
using Schedura.Application.Auth;
using Schedura.Application.Permissions;
using Schedura.Application.UserGroups;
using Schedura.Application.Users;
using Schedura.Application.Persons;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Auth;
using Schedura.Domain.Interfaces.Services.Permissions;
using Schedura.Domain.Interfaces.Services.Persons;
using Schedura.Domain.Interfaces.Services.UserGroups;
using Schedura.Domain.Interfaces.Services.Users;
using Schedura.Infra.DependencyInjection;
using Schedura.Services.Auth;
using Schedura.Services.Permissions;
using Schedura.Services.Persons;
using Schedura.Services.UserGroups;
using Schedura.Services.Users;

namespace Schedura.Bootstraper.DependencyInjection;

public static class ServiceCollectionExtensions {
	public static IServiceCollection AddSchedura(this IServiceCollection services, IConfiguration configuration) {
		services.AddAutoMapper(
			typeof(UserApplicationMappingProfile).Assembly,
			typeof(AuthApplicationMappingProfile).Assembly);

		// Application
		services.AddScoped<IPersonApplication, PersonApplication>();
		services.AddScoped<IUserApplication, UserApplication>();
		services.AddScoped<IAuthApplication, AuthApplication>();
		services.AddScoped<IUserGroupApplication, UserGroupApplication>();
		services.AddScoped<IPermissionApplication, PermissionApplication>();

		// Services
		services.AddScoped<IPersonService, PersonService>();
		services.AddScoped<IUserService, UserService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddScoped<IUserGroupService, UserGroupService>();
		services.AddScoped<IPermissionService, PermissionService>();

		// Validators
		services.AddScoped<IValidator<CreatePersonInput>, CreatePersonInputValidator>();
		services.AddScoped<IValidator<CreateUserInput>, CreateUserInputValidator>();
		services.AddScoped<IValidator<UpdateUserInput>, UpdateUserInputValidator>();
		services.AddScoped<IValidator<LoginInput>, LoginInputValidator>();
		services.AddScoped<IValidator<CreateUserGroupInput>, CreateUserGroupInputValidator>();
		services.AddScoped<IValidator<UpdateUserGroupInput>, UpdateUserGroupInputValidator>();
		services.AddScoped<IValidator<CreatePermissionInput>, CreatePermissionInputValidator>();
		services.AddScoped<IValidator<UpdatePermissionInput>, UpdatePermissionInputValidator>();

		services.AddInfrastructure(configuration);

		return services;
	}
}
