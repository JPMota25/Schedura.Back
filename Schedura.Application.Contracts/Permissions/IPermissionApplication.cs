namespace Schedura.Application.Contracts.Permissions;

public interface IPermissionApplication {
	Task<PermissionResponse> CreateAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<PermissionResponse>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<PermissionResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<UpdatePermissionResponse> UpdateAsync(string id, UpdatePermissionRequest request, CancellationToken cancellationToken = default);
	Task<DeletePermissionResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
