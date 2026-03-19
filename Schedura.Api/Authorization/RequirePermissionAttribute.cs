using Microsoft.AspNetCore.Authorization;

namespace Schedura.Api.Authorization;

public class RequirePermissionAttribute(string action)
	: AuthorizeAttribute, IAuthorizationRequirementData {

	public IEnumerable<IAuthorizationRequirement> GetRequirements() {
		yield return new PermissionRequirement(action);
	}
}
