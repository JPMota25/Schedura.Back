namespace Schedura.Domain.Interfaces.Services.Users;

public interface IUserService {
	Task<UserResult> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<UserResult>> GetAllAsync(GetAllUsersParams @params, CancellationToken cancellationToken = default);
	Task<UserResult?> GetByIdAsync(GetUserByIdParams @params, CancellationToken cancellationToken = default);
	Task<UpdateUserResult> UpdateAsync(UpdateUserInput input, CancellationToken cancellationToken = default);
	Task<DeleteUserResult> DeleteAsync(DeleteUserInput input, CancellationToken cancellationToken = default);
}
