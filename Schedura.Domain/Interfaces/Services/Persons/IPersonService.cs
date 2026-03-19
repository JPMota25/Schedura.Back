namespace Schedura.Domain.Interfaces.Services.Persons;

public interface IPersonService {
	Task<PersonResult> CreateAsync(CreatePersonInput input, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<PersonResult>> GetByFiltersAsync(GetPersonByFiltersParams @params, CancellationToken cancellationToken = default);
}
