using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Permissions;

namespace Schedura.Services.Permissions;

public class PermissionService(
	IPermissionRepository permissionRepository,
	IValidator<CreatePermissionInput> createValidator,
	IValidator<UpdatePermissionInput> updateValidator) : IPermissionService {

	public async Task<PermissionResult> CreateAsync(CreatePermissionInput input, CancellationToken cancellationToken = default) {
		await createValidator.ValidateAndThrowAsync(input, cancellationToken);

		var permission = new Permission(input.Action, input.Name, input.Description);
		await permissionRepository.CreateAsync(permission, cancellationToken);
		return ToResult(permission);
	}

	public async Task<IReadOnlyList<PermissionResult>> GetAllAsync(CancellationToken cancellationToken = default) {
		var permissions = await permissionRepository.GetAllAsNoTrackingAsync(cancellationToken);
		return permissions.Select(ToResult).ToList();
	}

	public async Task<PermissionResult?> GetByIdAsync(GetPermissionByIdParams @params, CancellationToken cancellationToken = default) {
		var permission = await permissionRepository.GetByIdAsNoTrackingAsync(@params.Id, cancellationToken);
		return permission is null ? null : ToResult(permission);
	}

	public async Task<UpdatePermissionResult> UpdateAsync(UpdatePermissionInput input, CancellationToken cancellationToken = default) {
		var permission = await permissionRepository.GetByIdAsync(input.Id, cancellationToken);
		if (permission is null) return new UpdatePermissionResult(false);

		await updateValidator.ValidateAndThrowAsync(input, cancellationToken);

		permission.SetAction(input.Action);
		permission.SetName(input.Name);
		permission.SetDescription(input.Description);
		await permissionRepository.UpdateAsync(permission, cancellationToken);
		return new UpdatePermissionResult(true);
	}

	public async Task<DeletePermissionResult> DeleteAsync(DeletePermissionInput input, CancellationToken cancellationToken = default) {
		var permission = await permissionRepository.GetByIdAsync(input.Id, cancellationToken);
		if (permission is null) return new DeletePermissionResult(false);

		await permissionRepository.DeleteAsync(permission, cancellationToken);
		return new DeletePermissionResult(true);
	}

	private static PermissionResult ToResult(Permission permission) =>
		new(permission.Id, permission.Action, permission.Name, permission.Description, permission.CreatedAt);
}
