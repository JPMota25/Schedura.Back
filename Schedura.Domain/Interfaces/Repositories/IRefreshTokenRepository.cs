using Schedura.Domain.Entities;

namespace Schedura.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken, string> {
	Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
	Task RevokeAllForUserAsync(string userId, CancellationToken cancellationToken = default);
}
