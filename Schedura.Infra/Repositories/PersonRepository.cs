using Microsoft.EntityFrameworkCore;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Infra.Data;

namespace Schedura.Infra.Repositories;

public class PersonRepository(AppDbContext context)
	: GenericRepository<Person, string>(context), IPersonRepository {

	public async Task<IReadOnlyList<Person>> GetByFiltersAsync(
		string? search, int limit, CancellationToken cancellationToken = default) {

		var query = context.Persons.Where(p => p.DeletedAt == null);

		if (!string.IsNullOrWhiteSpace(search))
			query = query.Where(p =>
				(p.Name + " " + p.Surname).ToLower().Contains(search.ToLower()) ||
				p.Email.ToLower().Contains(search.ToLower()));

		return await query.OrderBy(p => p.Name).Take(limit).ToListAsync(cancellationToken);
	}
}
