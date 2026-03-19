using Schedura.Domain.Entities;

namespace Schedura.Domain.Interfaces.Repositories;

public interface IPermissionRepository : IGenericRepository<Permission, string> {
	Task<Permission?> GetByActionAsync(string action, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<string>> GetActionsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
