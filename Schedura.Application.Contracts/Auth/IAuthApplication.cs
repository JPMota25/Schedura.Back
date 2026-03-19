namespace Schedura.Application.Contracts.Auth;

public interface IAuthApplication {
	Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
	Task<RefreshResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default);
	Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
}
