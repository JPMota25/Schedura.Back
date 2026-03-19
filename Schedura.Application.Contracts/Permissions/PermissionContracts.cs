using System.ComponentModel.DataAnnotations;

namespace Schedura.Application.Contracts.Permissions;

public record CreatePermissionRequest(
	[Required, StringLength(80)] string Action,
	[Required, StringLength(120)] string Name,
	[StringLength(255)] string? Description);

public record UpdatePermissionRequest(
	[Required, StringLength(80)] string Action,
	[Required, StringLength(120)] string Name,
	[StringLength(255)] string? Description);

public record PermissionResponse(
	string Id,
	string Action,
	string Name,
	string? Description,
	DateTimeOffset CreatedAt);

public record UpdatePermissionResponse(bool Found);
public record DeletePermissionResponse(bool Found);
