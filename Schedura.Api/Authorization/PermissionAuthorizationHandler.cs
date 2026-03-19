using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Schedura.Domain.Interfaces.Repositories;

namespace Schedura.Api.Authorization;

public class PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory)
	: AuthorizationHandler<PermissionRequirement> {

	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		PermissionRequirement requirement) {

		var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
		if (string.IsNullOrEmpty(userId)) {
			context.Fail();
			return;
		}

		using var scope = scopeFactory.CreateScope();
		var permissionRepository = scope.ServiceProvider.GetRequiredService<IPermissionRepository>();

		var actions = await permissionRepository.GetActionsByUserIdAsync(userId);

		if (actions.Contains(requirement.Action)) {
			context.Succeed(requirement);
		}
		else {
			context.Fail();
		}
	}
}
