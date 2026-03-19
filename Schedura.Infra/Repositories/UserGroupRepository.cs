using Microsoft.EntityFrameworkCore;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Infra.Data;

namespace Schedura.Infra.Repositories;

public class UserGroupRepository(AppDbContext context)
	: GenericRepository<UserGroup, string>(context), IUserGroupRepository {

	private readonly AppDbContext _context = context;

	public async Task<UserGroup?> GetByIdWithPermissionsAsync(string id, CancellationToken cancellationToken = default) {
		return await _context.UserGroups
			.Include(g => g.Permissions)
				.ThenInclude(p => p.Permission)
			.Where(g => g.DeletedAt == null && g.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<UserGroup?> GetByIdWithUsersAsync(string id, CancellationToken cancellationToken = default) {
		return await _context.UserGroups
			.Include(g => g.UserGroups)
				.ThenInclude(ug => ug.User)
			.Where(g => g.DeletedAt == null && g.Id == id)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<UserGroup>> GetAllWithPermissionsAsync(CancellationToken cancellationToken = default) {
		return await _context.UserGroups
			.Include(g => g.Permissions)
				.ThenInclude(p => p.Permission)
			.Where(g => g.DeletedAt == null)
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}
}
