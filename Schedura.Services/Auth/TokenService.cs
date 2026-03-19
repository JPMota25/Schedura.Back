using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Schedura.Domain.Interfaces.Services.Auth;

namespace Schedura.Services.Auth;

public class TokenService(IConfiguration configuration) : ITokenService {
	private readonly string _secret = configuration["Jwt:Secret"]
		?? throw new InvalidOperationException("Jwt:Secret não configurado.");
	private readonly string _issuer = configuration["Jwt:Issuer"] ?? "schedura";
	private readonly string _audience = configuration["Jwt:Audience"] ?? "schedura";
	private readonly int _expirationMinutes = int.Parse(configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");

	public string GenerateAccessToken(string userId, string username) {
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim> {
			new(JwtRegisteredClaimNames.Sub, userId),
			new(JwtRegisteredClaimNames.UniqueName, username),
			new(JwtRegisteredClaimNames.Iat,
				DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
				ClaimValueTypes.Integer64),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		};

		var token = new JwtSecurityToken(
			issuer: _issuer,
			audience: _audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public string? ValidateAccessToken(string token) {
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
		var handler = new JwtSecurityTokenHandler();

		try {
			var principal = handler.ValidateToken(token, new TokenValidationParameters {
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = _issuer,
				ValidAudience = _audience,
				IssuerSigningKey = key,
				ClockSkew = TimeSpan.Zero,
			}, out _);

			return principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
		}
		catch {
			return null;
		}
	}

	public string GenerateRefreshToken() {
		var bytes = RandomNumberGenerator.GetBytes(64);
		return Convert.ToBase64String(bytes);
	}
}
