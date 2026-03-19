using Microsoft.EntityFrameworkCore;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Infra.Data;

namespace Schedura.Infra.Repositories;

public class RefreshTokenRepository(AppDbContext context)
	: GenericRepository<RefreshToken, string>(context), IRefreshTokenRepository {

	private readonly AppDbContext _context = context;

	public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default) {
		return await _context.RefreshTokens
			.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
	}

	public async Task RevokeAllForUserAsync(string userId, CancellationToken cancellationToken = default) {
		var tokens = await _context.RefreshTokens
			.Where(rt => rt.UserId == userId && rt.RevokedAt == null)
			.ToListAsync(cancellationToken);

		foreach (var token in tokens) {
			token.Revoke();
			_context.RefreshTokens.Update(token);
		}
	}
}
