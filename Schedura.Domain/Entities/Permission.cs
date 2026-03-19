namespace Schedura.Domain.Entities;

public class Permission(string action, string name, string? description = null) : BaseEntity {
	public string Action { get; private set; } = action;
	public string Name { get; private set; } = name;
	public string? Description { get; private set; } = description;

	public ICollection<UserGroupPermission> UserGroupPermissions { get; private set; } = [];

	public void SetAction(string action) {
		Action = action;
		SetUpdatedAt();
	}

	public void SetName(string name) {
		Name = name;
		SetUpdatedAt();
	}

	public void SetDescription(string? description) {
		Description = description;
		SetUpdatedAt();
	}

	private Permission() : this(string.Empty, string.Empty) { }
}
