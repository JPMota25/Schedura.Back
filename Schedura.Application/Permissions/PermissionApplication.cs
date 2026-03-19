using AutoMapper;
using Schedura.Application.Contracts.Permissions;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Permissions;

namespace Schedura.Application.Permissions;

public class PermissionApplication(
	IPermissionService permissionService,
	IUnitOfWork unitOfWork,
	IMapper mapper) : IPermissionApplication {

	public async Task<PermissionResponse> CreateAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var input = mapper.Map<CreatePermissionInput>(request);
			var result = await permissionService.CreateAsync(input, ct);
			return mapper.Map<PermissionResponse>(result);
		}, cancellationToken);
	}

	public async Task<IReadOnlyList<PermissionResponse>> GetAllAsync(CancellationToken cancellationToken = default) {
		var results = await permissionService.GetAllAsync(cancellationToken);
		return results.Select(r => mapper.Map<PermissionResponse>(r)).ToList();
	}

	public async Task<PermissionResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default) {
		var result = await permissionService.GetByIdAsync(new GetPermissionByIdParams(id), cancellationToken);
		return result is null ? null : mapper.Map<PermissionResponse>(result);
	}

	public async Task<UpdatePermissionResponse> UpdateAsync(string id, UpdatePermissionRequest request, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var input = new UpdatePermissionInput(id, request.Action, request.Name, request.Description);
			var result = await permissionService.UpdateAsync(input, ct);
			return mapper.Map<UpdatePermissionResponse>(result);
		}, cancellationToken);
	}

	public async Task<DeletePermissionResponse> DeleteAsync(string id, CancellationToken cancellationToken = default) {
		return await ExecuteInTransactionAsync(async ct => {
			var result = await permissionService.DeleteAsync(new DeletePermissionInput(id), ct);
			return mapper.Map<DeletePermissionResponse>(result);
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
