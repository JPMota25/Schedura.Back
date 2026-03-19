using Schedura.Domain.Entities;

namespace Schedura.Domain.Interfaces.Repositories;

public interface IPersonRepository : IGenericRepository<Person, string> {
	Task<IReadOnlyList<Person>> GetByFiltersAsync(string? search, int limit, CancellationToken cancellationToken = default);
}
