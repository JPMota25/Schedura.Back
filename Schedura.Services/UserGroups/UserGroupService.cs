using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.UserGroups;

namespace Schedura.Services.UserGroups;

public class UserGroupService(
	IUserGroupRepository userGroupRepository,
	IPermissionRepository permissionRepository,
	IUserRepository userRepository,
	IValidator<CreateUserGroupInput> createValidator,
	IValidator<UpdateUserGroupInput> updateValidator) : IUserGroupService {

	public async Task<UserGroupResult> CreateAsync(CreateUserGroupInput input, CancellationToken cancellationToken = default) {
		await createValidator.ValidateAndThrowAsync(input, cancellationToken);

		var group = new UserGroup(input.Name, input.Description);
		await userGroupRepository.CreateAsync(group, cancellationToken);
		return ToResult(group);
	}

	public async Task<IReadOnlyList<UserGroupResult>> GetAllAsync(CancellationToken cancellationToken = default) {
		var groups = await userGroupRepository.GetAllWithPermissionsAsync(cancellationToken);
		return groups.Select(g => ToResult(g, includePermissions: true)).ToList();
	}

	public async Task<UserGroupResult?> GetByIdAsync(GetUserGroupByIdParams @params, CancellationToken cancellationToken = default) {
		var group = await userGroupRepository.GetByIdWithPermissionsAsync(@params.Id, cancellationToken);
		return group is null ? null : ToResult(group, includePermissions: true);
	}

	public async Task<UpdateUserGroupResult> UpdateAsync(UpdateUserGroupInput input, CancellationToken cancellationToken = default) {
		var group = await userGroupRepository.GetByIdAsync(input.Id, cancellationToken);
		if (group is null) return new UpdateUserGroupResult(false);

		await updateValidator.ValidateAndThrowAsync(input, cancellationToken);

		group.SetName(input.Name);
		group.SetDescription(input.Description);
		await userGroupRepository.UpdateAsync(group, cancellationToken);
		return new UpdateUserGroupResult(true);
	}

	public async Task<DeleteUserGroupResult> DeleteAsync(DeleteUserGroupInput input, CancellationToken cancellationToken = default) {
		var group = await userGroupRepository.GetByIdAsync(input.Id, cancellationToken);
		if (group is null) return new DeleteUserGroupResult(false);

		await userGroupRepository.DeleteAsync(group, cancellationToken);
		return new DeleteUserGroupResult(true);
	}

	public async Task AddPermissionAsync(string userGroupId, string permissionId, CancellationToken cancellationToken = default) {
		var group = await userGroupRepository.GetByIdWithPermissionsAsync(userGroupId, cancellationToken)
			?? throw new InvalidOperationException("Grupo não encontrado.");

		var permissionExists = await permissionRepository.GetByIdAsync(permissionId, cancellationToken)
			?? throw new InvalidOperationException("Permissão não encontrada.");

		if (group.Permissions.Any(p => p.PermissionId == permissionId)) return;

		var link = new UserGroupPermission(userGroupId, permissionId);
		group.Permissions.Add(link);
		await userGroupRepository.UpdateAsync(group, cancellationToken);
	}

	public async Task RemovePermissionAsync(string userGroupId, string permissionId, CancellationToken cancellationToken = default) {
		var group = await userGroupRepository.GetByIdWithPermissionsAsync(userGroupId, cancellationToken)
			?? throw new InvalidOperationException("Grupo não encontrado.");

		var link = group.Permissions.FirstOrDefault(p => p.PermissionId == permissionId);
		if (link is null) return;

		group.Permissions.Remove(link);
		await userGroupRepository.UpdateAsync(group, cancellationToken);
	}

	public async Task AddUserAsync(string userGroupId, string userId, CancellationToken cancellationToken = default) {
		var group = await userGroupRepository.GetByIdWithUsersAsync(userGroupId, cancellationToken)
			?? throw new InvalidOperationException("Grupo não encontrado.");

		var user = await userRepository.GetByIdAsync(userId, cancellationToken)
			?? throw new InvalidOperationException("Usuário não encontrado.");

		if (group.UserGroups.Any(ug => ug.UserId == userId)) return;

		var link = new UserUserGroup(userId, userGroupId);
		group.UserGroups.Add(link);
		await userGroupRepository.UpdateAsync(group, cancellationToken);
	}

	public async Task RemoveUserAsync(string userGroupId, string userId, CancellationToken cancellationToken = default) {
		var group = await userGroupRepository.GetByIdWithUsersAsync(userGroupId, cancellationToken)
			?? throw new InvalidOperationException("Grupo não encontrado.");

		var link = group.UserGroups.FirstOrDefault(ug => ug.UserId == userId);
		if (link is null) return;

		group.UserGroups.Remove(link);
		await userGroupRepository.UpdateAsync(group, cancellationToken);
	}

	private static UserGroupResult ToResult(UserGroup group, bool includePermissions = false) {
		var permissionActions = includePermissions
			? group.Permissions.Where(p => p.Permission is not null).Select(p => p.Permission!.Action).ToList()
			: null;
		var userIds = group.UserGroups.Any()
			? group.UserGroups.Select(ug => ug.UserId).ToList()
			: null;

		return new UserGroupResult(
			group.Id,
			group.Name,
			group.Description,
			group.CreatedAt,
			group.UpdatedAt,
			permissionActions,
			userIds);
	}
}
