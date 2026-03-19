namespace Schedura.Application.Contracts.UserGroups;

public interface IUserGroupApplication {
	Task<UserGroupResponse> CreateAsync(CreateUserGroupRequest request, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<UserGroupResponse>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<UserGroupResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<UpdateUserGroupResponse> UpdateAsync(string id, UpdateUserGroupRequest request, CancellationToken cancellationToken = default);
	Task<DeleteUserGroupResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);
	Task AddPermissionAsync(string id, AddPermissionToGroupRequest request, CancellationToken cancellationToken = default);
	Task RemovePermissionAsync(string id, string permissionId, CancellationToken cancellationToken = default);
	Task AddUserAsync(string id, AddUserToGroupRequest request, CancellationToken cancellationToken = default);
	Task RemoveUserAsync(string id, string userId, CancellationToken cancellationToken = default);
}
