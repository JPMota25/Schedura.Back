using Schedura.Domain.Entities;

namespace Schedura.Domain.Interfaces.Repositories;

public interface IUserGroupRepository : IGenericRepository<UserGroup, string> {
	Task<UserGroup?> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken = default);
	Task<UserGroup?> GetByIdWithUsersAsync(string id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<UserGroup>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default);
}
