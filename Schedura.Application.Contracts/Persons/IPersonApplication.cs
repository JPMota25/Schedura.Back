namespace Schedura.Application.Contracts.Persons;

public interface IPersonApplication {
	Task<PersonResponse> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<PersonResponse>> GetByFiltersAsync(GetPersonByFiltersRequest request, CancellationToken cancellationToken = default);
}
