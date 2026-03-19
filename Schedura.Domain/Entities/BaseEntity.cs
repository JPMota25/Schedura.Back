namespace Schedura.Domain.Entities;

public abstract class BaseEntity {
	public string Id { get; private set; } = Guid.NewGuid().ToString().Replace("-", "");
	public DateTimeOffset CreatedAt { get; private set; } = DateTime.UtcNow;
	public DateTimeOffset? UpdatedAt { get; private set; }
	public DateTimeOffset? DeletedAt { get; private set; }

	public void SetUpdatedAt() {
		UpdatedAt = DateTime.UtcNow;
	}

	public void SetDeletedAt() {
		DeletedAt = DateTime.UtcNow;
	}
}
