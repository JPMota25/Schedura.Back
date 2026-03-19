using FluentValidation;
using Microsoft.Extensions.Configuration;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Auth;
using Schedura.Services.Users;

namespace Schedura.Services.Auth;

public class AuthService(
	IUserRepository userRepository,
	IRefreshTokenRepository refreshTokenRepository,
	IPermissionRepository permissionRepository,
	ITokenService tokenService,
	IValidator<LoginInput> loginValidator,
	IConfiguration configuration) : IAuthService {

	private readonly int _refreshTokenDays = int.Parse(
		configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");

	public async Task<LoginServiceResult> LoginAsync(LoginInput input, CancellationToken cancellationToken = default) {
		await loginValidator.ValidateAndThrowAsync(input, cancellationToken);

		var user = await userRepository.GetByUsernameAsync(input.Username, cancellationToken);

		if (user is null || !PasswordHasher.Verify(input.Password, user.Password)) {
			throw new UnauthorizedAccessException("Credenciais inválidas.");
		}

		var permissions = await permissionRepository.GetActionsByUserIdAsync(user.Id, cancellationToken);
		var accessToken = tokenService.GenerateAccessToken(user.Id, user.Username);
		var refreshTokenValue = tokenService.GenerateRefreshToken();

		var expiresAt = DateTimeOffset.UtcNow.AddDays(_refreshTokenDays);
		var refreshToken = new RefreshToken(refreshTokenValue, user.Id, expiresAt);
		await refreshTokenRepository.CreateAsync(refreshToken, cancellationToken);

		var expiresIn = 15 * 60;

		return new LoginServiceResult(accessToken, refreshTokenValue, expiresIn, user.Id, user.Username, permissions);
	}

	public async Task<RefreshServiceResult> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default) {
		var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

		if (token is null || !token.IsActive) {
			throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");
		}

		var user = await userRepository.GetByIdAsNoTrackingAsync(token.UserId, cancellationToken);
		if (user is null) {
			throw new UnauthorizedAccessException("Usuário não encontrado.");
		}

		token.Revoke();
		await refreshTokenRepository.UpdateAsync(token, cancellationToken);

		var permissions = await permissionRepository.GetActionsByUserIdAsync(user.Id, cancellationToken);
		var newAccessToken = tokenService.GenerateAccessToken(user.Id, user.Username);
		var newRefreshTokenValue = tokenService.GenerateRefreshToken();

		var expiresAt = DateTimeOffset.UtcNow.AddDays(_refreshTokenDays);
		var newRefreshToken = new RefreshToken(newRefreshTokenValue, user.Id, expiresAt);
		await refreshTokenRepository.CreateAsync(newRefreshToken, cancellationToken);

		var expiresIn = 15 * 60;

		return new RefreshServiceResult(newAccessToken, newRefreshTokenValue, expiresIn, permissions);
	}

	public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default) {
		var token = await refreshTokenRepository.GetByTokenAsync(refreshToken, cancellationToken);

		if (token is null || token.IsRevoked) return;

		token.Revoke();
		await refreshTokenRepository.UpdateAsync(token, cancellationToken);
	}

	public async Task LogoutAllAsync(string userId, CancellationToken cancellationToken = default) {
		await refreshTokenRepository.RevokeAllForUserAsync(userId, cancellationToken);
	}
}
