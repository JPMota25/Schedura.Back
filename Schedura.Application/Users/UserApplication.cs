using AutoMapper;
using Schedura.Application.Contracts.Users;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Persons;
using Schedura.Domain.Interfaces.Services.Users;

namespace Schedura.Application.Users;

public class UserApplication(
	IPersonService personService,
	IUserService userService,
	IUnitOfWork unitOfWork,
	IMapper mapper) : IUserApplication {
	public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var personInput = mapper.Map<CreatePersonInput>(request.Person);
			var person = await personService.CreateAsync(personInput, ct);

			var input = mapper.Map<CreateUserInput>(request, opts => opts.Items["personId"] = person.Id);
			var result = await userService.CreateAsync(input, ct);
			return mapper.Map<UserResponse>(result);
		}, cancellationToken);
	}

	public async Task<IReadOnlyList<UserResponse>> GetAllAsync(GetAllUsersRequest request, CancellationToken cancellationToken = default) {
		var @params = mapper.Map<GetAllUsersParams>(request);
		var results = await userService.GetAllAsync(@params, cancellationToken);
		return mapper.Map<IReadOnlyList<UserResponse>>(results);
	}

	public async Task<UserResponse?> GetByIdAsync(GetUserByIdRequest request, CancellationToken cancellationToken = default) {
		var @params = mapper.Map<GetUserByIdParams>(request);
		var result = await userService.GetByIdAsync(@params, cancellationToken);
		return result is null ? null : mapper.Map<UserResponse>(result);
	}

	public async Task<UpdateUserResponse> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var input = mapper.Map<UpdateUserInput>(request, opts => opts.Items["id"] = id);
			var result = await userService.UpdateAsync(input, ct);
			return mapper.Map<UpdateUserResponse>(result);
		}, cancellationToken);
	}

	public async Task<DeleteUserResponse> DeleteAsync(DeleteUserRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var input = mapper.Map<DeleteUserInput>(request);
			var result = await userService.DeleteAsync(input, ct);
			return mapper.Map<DeleteUserResponse>(result);
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
