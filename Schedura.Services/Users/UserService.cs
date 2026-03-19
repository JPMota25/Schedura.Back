using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Common;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Users;

namespace Schedura.Services.Users;

public class UserService(
	IUserRepository userRepository,
	IValidator<CreateUserInput> createUserValidator,
	IValidator<UpdateUserInput> updateUserValidator) : IUserService {
	public async Task<UserResult> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default) {
		await createUserValidator.ValidateAndThrowAsync(input, cancellationToken);

		var hashedPassword = PasswordHasher.Hash(input.Password);
		var user = new User(input.Username, hashedPassword, input.PersonId);
		await userRepository.CreateAsync(user, cancellationToken);
		return ToResult(user);
	}

	public async Task<IReadOnlyList<UserResult>> GetAllAsync(GetAllUsersParams @params, CancellationToken cancellationToken = default) {
		var users = await userRepository.GetAllAsNoTrackingAsync(cancellationToken);
		return users.Select(ToResult).ToList();
	}

	public async Task<UserResult?> GetByIdAsync(GetUserByIdParams @params, CancellationToken cancellationToken = default) {
		var user = await userRepository.GetByIdAsNoTrackingAsync(@params.Id, cancellationToken);
		return user is null ? null : ToResult(user);
	}

	public async Task<UpdateUserResult> UpdateAsync(UpdateUserInput input, CancellationToken cancellationToken = default) {
		var user = await userRepository.GetByIdAsync(input.Id, cancellationToken);
		if (user is null) {
			return new UpdateUserResult(false);
		}

		await updateUserValidator.ValidateAndThrowAsync(input, cancellationToken);

		user.SetUsername(input.Username);
		user.SetPassword(PasswordHasher.Hash(input.Password));
		user.SetPersonId(input.PersonId);

		await userRepository.UpdateAsync(user, cancellationToken);
		return new UpdateUserResult(true);
	}

	public async Task<DeleteUserResult> DeleteAsync(DeleteUserInput input, CancellationToken cancellationToken = default) {
		var user = await userRepository.GetByIdAsync(input.Id, cancellationToken);
		if (user is null) {
			return new DeleteUserResult(false);
		}

		await userRepository.DeleteAsync(user, cancellationToken);
		return new DeleteUserResult(true);
	}

	public Task<PagedResult<UserResult>> GetUsersReportByUiFiltersAsync(GetUsersReportByUiFiltersParams @params, CancellationToken cancellationToken = default) {
		return userRepository.GetUsersReportByUiFilters(@params.Query, cancellationToken);
	}

	private static UserResult ToResult(User user) {
		return new UserResult(user.Id, user.Username, user.PersonId, user.CreatedAt, user.UpdatedAt);
	}
}
