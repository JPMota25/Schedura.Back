using Microsoft.AspNetCore.Authorization;

namespace Schedura.Api.Authorization;

public class PermissionRequirement(string action) : IAuthorizationRequirement {
	public string Action { get; } = action;
}
