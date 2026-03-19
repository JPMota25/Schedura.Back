namespace Schedura.Domain.Interfaces.Services.Auth;

public record LoginInput(string Username, string Password);
public record LoginServiceResult(
	string AccessToken,
	string RefreshToken,
	int ExpiresIn,
	string UserId,
	string Username,
	IReadOnlyList<string> Permissions);
public record RefreshServiceResult(
	string AccessToken,
	string RefreshToken,
	int ExpiresIn,
	IReadOnlyList<string> Permissions);

public interface IAuthService {
	Task<LoginServiceResult> LoginAsync(LoginInput input, CancellationToken cancellationToken = default);
	Task<RefreshServiceResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);
	Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
	Task LogoutAllAsync(string userId, CancellationToken cancellationToken = default);
}
