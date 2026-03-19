namespace Schedura.Domain.Entities;

public class UserUserGroup {
	public string UserId { get; private set; } = string.Empty;
	public string UserGroupId { get; private set; } = string.Empty;
	public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

	public User? User { get; private set; }
	public UserGroup? Group { get; private set; }

	public UserUserGroup(string userId, string userGroupId) {
		UserId = userId;
		UserGroupId = userGroupId;
	}

	private UserUserGroup() { }
}
