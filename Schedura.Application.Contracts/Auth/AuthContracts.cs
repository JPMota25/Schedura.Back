using System.ComponentModel.DataAnnotations;

namespace Schedura.Application.Contracts.Auth;

public record LoginRequest(
	[Required] string Username,
	[Required] string Password);

public record LoginResponse(
	string AccessToken,
	string RefreshToken,
	int ExpiresIn,
	string UserId,
	string Username,
	IReadOnlyList<string> Permissions);

public record RefreshRequest([Required] string RefreshToken);

public record RefreshResponse(
	string AccessToken,
	string RefreshToken,
	int ExpiresIn,
	IReadOnlyList<string> Permissions);

public record LogoutRequest([Required] string RefreshToken);
