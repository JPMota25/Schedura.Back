namespace Schedura.Domain.Interfaces.Services.Auth;

public interface ITokenService {
	string GenerateAccessToken(string userId, string username);
	string? ValidateAccessToken(string token);
	string GenerateRefreshToken();
}
