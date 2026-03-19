using Microsoft.EntityFrameworkCore;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Infra.Data;

namespace Schedura.Infra.Repositories;

public class PermissionRepository(AppDbContext context)
	: GenericRepository<Permission, string>(context), IPermissionRepository {

	private readonly AppDbContext _context = context;

	public async Task<Permission?> GetByActionAsync(string action, CancellationToken cancellationToken = default) {
		return await _context.Permissions
			.Where(p => p.DeletedAt == null && p.Action == action)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<string>> GetActionsByUserIdAsync(string userId, CancellationToken cancellationToken = default) {
		return await _context.UserUserGroups
			.Where(uug => uug.UserId == userId)
			.SelectMany(uug => uug.Group!.Permissions)
			.Select(ugp => ugp.Permission!.Action)
			.Distinct()
			.ToListAsync(cancellationToken);
	}
}
