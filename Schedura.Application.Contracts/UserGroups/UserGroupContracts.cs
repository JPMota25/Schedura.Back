using System.ComponentModel.DataAnnotations;

namespace Schedura.Application.Contracts.UserGroups;

public record CreateUserGroupRequest(
	[Required, StringLength(80)] string Name,
	[StringLength(255)] string? Description);

public record UpdateUserGroupRequest(
	[Required, StringLength(80)] string Name,
	[StringLength(255)] string? Description);

public record UserGroupResponse(
	string Id,
	string Name,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset? UpdatedAt,
	IReadOnlyList<string>? PermissionActions,
	IReadOnlyList<string>? UserIds);

public record UpdateUserGroupResponse(bool Found);
public record DeleteUserGroupResponse(bool Found);
public record AddPermissionToGroupRequest([Required] string PermissionId);
public record AddUserToGroupRequest([Required] string UserId);
