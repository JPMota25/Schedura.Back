namespace Schedura.Domain.Interfaces.Services.UserGroups;

public record CreateUserGroupInput(string Name, string? Description);
public record UpdateUserGroupInput(string Id, string Name, string? Description);
public record DeleteUserGroupInput(string Id);
public record GetUserGroupByIdParams(string Id);
public record UserGroupResult(
	string Id,
	string Name,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset? UpdatedAt,
	IReadOnlyList<string>? PermissionActions = null,
	IReadOnlyList<string>? UserIds = null);
public record UpdateUserGroupResult(bool Found);
public record DeleteUserGroupResult(bool Found);

public interface IUserGroupService {
	Task<UserGroupResult> CreateAsync(CreateUserGroupInput input, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<UserGroupResult>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<UserGroupResult?> GetByIdAsync(GetUserGroupByIdParams @params, CancellationToken cancellationToken = default);
	Task<UpdateUserGroupResult> UpdateAsync(UpdateUserGroupInput input, CancellationToken cancellationToken = default);
	Task<DeleteUserGroupResult> DeleteAsync(DeleteUserGroupInput input, CancellationToken cancellationToken = default);
	Task AddPermissionAsync(string userGroupId, string permissionId, CancellationToken cancellationToken = default);
	Task RemovePermissionAsync(string userGroupId, string permissionId, CancellationToken cancellationToken = default);
	Task AddUserAsync(string userGroupId, string userId, CancellationToken cancellationToken = default);
	Task RemoveUserAsync(string userGroupId, string userId, CancellationToken cancellationToken = default);
}
