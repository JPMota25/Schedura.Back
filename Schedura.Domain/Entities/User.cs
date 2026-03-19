namespace Schedura.Domain.Entities;

public class User(string username, string password, string personId) : BaseEntity {
	public string Username { get; private set; } = username;
	public string Password { get; private set; } = password;
	public Person? Person { get; private set; }
	public string PersonId { get; private set; } = personId;

	public ICollection<UserUserGroup> Groups { get; private set; } = [];
	public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

	public void SetUsername(string username) {
		Username = username;
		SetUpdatedAt();
	}

	public void SetPassword(string password) {
		Password = password;
		SetUpdatedAt();
	}

	public void SetPersonId(string personId) {
		PersonId = personId;
		SetUpdatedAt();
	}

	private User() : this(string.Empty, string.Empty, string.Empty) { }
}
