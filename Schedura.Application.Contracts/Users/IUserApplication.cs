namespace Schedura.Application.Contracts.Users;

public interface IUserApplication {
	Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<UserResponse>> GetAllAsync(GetAllUsersRequest request, CancellationToken cancellationToken = default);
	Task<UserResponse?> GetByIdAsync(GetUserByIdRequest request, CancellationToken cancellationToken = default);
	Task<UpdateUserResponse> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
	Task<DeleteUserResponse> DeleteAsync(DeleteUserRequest request, CancellationToken cancellationToken = default);
	Task<SearchUsersResponse> SearchAsync(SearchUsersRequest request, CancellationToken cancellationToken = default);
}
