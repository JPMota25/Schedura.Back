namespace Schedura.Domain.Interfaces.Services.Permissions;

public record CreatePermissionInput(string Action, string Name, string? Description);
public record UpdatePermissionInput(string Id, string Action, string Name, string? Description);
public record DeletePermissionInput(string Id);
public record GetPermissionByIdParams(string Id);
public record PermissionResult(string Id, string Action, string Name, string? Description, DateTimeOffset CreatedAt);
public record UpdatePermissionResult(bool Found);
public record DeletePermissionResult(bool Found);

public interface IPermissionService {
	Task<PermissionResult> CreateAsync(CreatePermissionInput input, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<PermissionResult>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<PermissionResult?> GetByIdAsync(GetPermissionByIdParams @params, CancellationToken cancellationToken = default);
	Task<UpdatePermissionResult> UpdateAsync(UpdatePermissionInput input, CancellationToken cancellationToken = default);
	Task<DeletePermissionResult> DeleteAsync(DeletePermissionInput input, CancellationToken cancellationToken = default);
}
