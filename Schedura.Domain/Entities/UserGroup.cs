namespace Schedura.Domain.Entities;

public class UserGroup(string name, string? description = null) : BaseEntity {
	public string Name { get; private set; } = name;
	public string? Description { get; private set; } = description;

	public ICollection<UserUserGroup> UserGroups { get; private set; } = [];
	public ICollection<UserGroupPermission> Permissions { get; private set; } = [];

	public void SetName(string name) {
		Name = name;
		SetUpdatedAt();
	}

	public void SetDescription(string? description) {
		Description = description;
		SetUpdatedAt();
	}

	private UserGroup() : this(string.Empty) { }
}
