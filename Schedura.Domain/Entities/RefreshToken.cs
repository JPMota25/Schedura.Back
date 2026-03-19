namespace Schedura.Domain.Entities;

public class RefreshToken(string token, string userId, DateTimeOffset expiresAt, string? deviceInfo = null) : BaseEntity {
	public string Token { get; private set; } = token;
	public string UserId { get; private set; } = userId;
	public DateTimeOffset ExpiresAt { get; private set; } = expiresAt;
	public DateTimeOffset? RevokedAt { get; private set; }
	public string? DeviceInfo { get; private set; } = deviceInfo;

	public User? User { get; private set; }

	public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
	public bool IsRevoked => RevokedAt is not null;
	public bool IsActive => !IsExpired && !IsRevoked;

	public void Revoke() {
		RevokedAt = DateTimeOffset.UtcNow;
	}

	private RefreshToken() : this(string.Empty, string.Empty, DateTimeOffset.UtcNow) { }
}
