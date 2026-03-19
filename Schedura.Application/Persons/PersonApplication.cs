using AutoMapper;
using Schedura.Application.Contracts.Persons;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Persons;

namespace Schedura.Application.Persons;

public class PersonApplication(
	IPersonService personService,
	IUnitOfWork unitOfWork,
	IMapper mapper) : IPersonApplication {
	public async Task<PersonResponse> CreateAsync(CreatePersonRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var input = mapper.Map<CreatePersonInput>(request);
			var result = await personService.CreateAsync(input, ct);
			return mapper.Map<PersonResponse>(result);
		}, cancellationToken);
	}

	private async Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken) {
		await unitOfWork.BeginTransactionAsync(cancellationToken);
		try {
			var result = await action(cancellationToken);
			await unitOfWork.CommitAsync(cancellationToken);
			return result;
		}
		catch {
			await unitOfWork.RollbackAsync(cancellationToken);
			throw;
		}
	}
}
