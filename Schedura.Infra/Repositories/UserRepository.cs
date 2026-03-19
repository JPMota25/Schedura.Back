using Microsoft.EntityFrameworkCore;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Common;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Users;
using Schedura.Infra.Common;
using Schedura.Infra.Data;

namespace Schedura.Infra.Repositories;

public class UserRepository(AppDbContext context)
	: GenericRepository<User, string>(context), IUserRepository {

	private readonly AppDbContext _context = context;

	public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) {
		return await _context.Users
			.Where(u => u.DeletedAt == null && u.Username == username)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<PagedResult<UserResult>> GetUserReportByUiFilters(PagedQuery query, CancellationToken cancellationToken = default) {
		var baseQuery = _context.Users
			.Where(u => u.DeletedAt == null)
			.AsNoTracking()
			.Select(u => new UserResult(u.Id, u.Username, u.PersonId, u.CreatedAt, u.UpdatedAt));

		return await baseQuery.ApplyUiFilters(query, cancellationToken);
	}
}
