using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Common;
using Schedura.Domain.Interfaces.Services.Users;

namespace Schedura.Domain.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User, string> {
	Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
	Task<PagedResult<UserResult>> GetUserReportByUiFilters(PagedQuery query, CancellationToken cancellationToken = default);
}
