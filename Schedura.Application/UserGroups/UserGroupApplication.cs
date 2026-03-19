using AutoMapper;
using Schedura.Application.Contracts.UserGroups;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.UserGroups;

namespace Schedura.Application.UserGroups;

public class UserGroupApplication(
	IUserGroupService userGroupService,
	IUnitOfWork unitOfWork,
	IMapper mapper) : IUserGroupApplication {

	public async Task<UserGroupResponse> CreateAsync(CreateUserGroupRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var input = mapper.Map<CreateUserGroupInput>(request);
			var result = await userGroupService.CreateAsync(input, ct);
			return mapper.Map<UserGroupResponse>(result);
		}, cancellationToken);
	}

	public async Task<IReadOnlyList<UserGroupResponse>> GetAllAsync(CancellationToken cancellationToken = default) {
		var results = await userGroupService.GetAllAsync(cancellationToken);
		return results.Select(r => mapper.Map<UserGroupResponse>(r)).ToList();
	}

	public async Task<UserGroupResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default) {
		var result = await userGroupService.GetByIdAsync(new GetUserGroupByIdParams(id), cancellationToken);
		return result is null ? null : mapper.Map<UserGroupResponse>(result);
	}

	public async Task<UpdateUserGroupResponse> UpdateAsync(string id, UpdateUserGroupRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var input = new UpdateUserGroupInput(id, request.Name, request.Description);
			var result = await userGroupService.UpdateAsync(input, ct);
			return mapper.Map<UpdateUserGroupResponse>(result);
		}, cancellationToken);
	}

	public async Task<DeleteUserGroupResponse> DeleteAsync(string id, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var result = await userGroupService.DeleteAsync(new DeleteUserGroupInput(id), ct);
			return mapper.Map<DeleteUserGroupResponse>(result);
		}, cancellationToken);
	}

	public async Task AddPermissionAsync(string id, AddPermissionToGroupRequest request, CancellationToken cancellationToken = default) {
		await ExecuteInTransactionAsync(async ct => {
			await userGroupService.AddPermissionAsync(id, request.PermissionId, ct);
			return true;
		}, cancellationToken);
	}

	public async Task RemovePermissionAsync(string id, string permissionId, CancellationToken cancellationToken = default) {
		await ExecuteInTransactionAsync(async ct => {
			await userGroupService.RemovePermissionAsync(id, permissionId, ct);
			return true;
		}, cancellationToken);
	}

	public async Task AddUserAsync(string id, AddUserToGroupRequest request, CancellationToken cancellationToken = default) {
		await ExecuteInTransactionAsync(async ct => {
			await userGroupService.AddUserAsync(id, request.UserId, ct);
			return true;
		}, cancellationToken);
	}

	public async Task RemoveUserAsync(string id, string userId, CancellationToken cancellationToken = default) {
		await ExecuteInTransactionAsync(async ct => {
			await userGroupService.RemoveUserAsync(id, userId, ct);
			return true;
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
