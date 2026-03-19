namespace Schedura.Domain.Entities;

public class UserGroupPermission {
	public string UserGroupId { get; private set; } = string.Empty;
	public string PermissionId { get; private set; } = string.Empty;

	public UserGroup? Group { get; private set; }
	public Permission? Permission { get; private set; }

	public UserGroupPermission(string userGroupId, string permissionId) {
		UserGroupId = userGroupId;
		PermissionId = permissionId;
	}

	private UserGroupPermission() { }
}
