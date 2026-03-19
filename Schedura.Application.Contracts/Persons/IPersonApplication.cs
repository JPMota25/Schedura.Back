namespace Schedura.Application.Contracts.Persons;

public interface IPersonApplication {
	Task<PersonResponse> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default);
}
