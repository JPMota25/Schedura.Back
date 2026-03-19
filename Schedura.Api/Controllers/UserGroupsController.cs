using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedura.Api.Authorization;
using Schedura.Api.Common;
using Schedura.Application.Contracts.UserGroups;

namespace Schedura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserGroupsController(IUserGroupApplication userGroupApplication) : ControllerBase {

	[HttpPost]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse<UserGroupResponse>>> Create(
		[FromBody] CreateUserGroupRequest request,
		CancellationToken cancellationToken) {
		var response = await userGroupApplication.CreateAsync(request, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<UserGroupResponse>.Ok(response));
	}

	[HttpGet]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse<IReadOnlyList<UserGroupResponse>>>> GetAll(CancellationToken cancellationToken) {
		var response = await userGroupApplication.GetAllAsync(cancellationToken);
		return Ok(ApiResponse<IReadOnlyList<UserGroupResponse>>.Ok(response));
	}

	[HttpGet("{id}")]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse<UserGroupResponse>>> GetById(
		[FromRoute] string id,
		CancellationToken cancellationToken) {
		var response = await userGroupApplication.GetByIdAsync(id, cancellationToken);
		if (response is null) return NotFound(ApiResponse<UserGroupResponse>.Fail("User group not found."));
		return Ok(ApiResponse<UserGroupResponse>.Ok(response));
	}

	[HttpPut("{id}")]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse>> Update(
		[FromRoute] string id,
		[FromBody] UpdateUserGroupRequest request,
		CancellationToken cancellationToken) {
		var response = await userGroupApplication.UpdateAsync(id, request, cancellationToken);
		if (!response.Found) return NotFound(ApiResponse.Fail("User group not found."));
		return Ok(ApiResponse.Empty());
	}

	[HttpDelete("{id}")]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse>> Delete(
		[FromRoute] string id,
		CancellationToken cancellationToken) {
		var response = await userGroupApplication.DeleteAsync(id, cancellationToken);
		if (!response.Found) return NotFound(ApiResponse.Fail("User group not found."));
		return Ok(ApiResponse.Empty());
	}

	[HttpPost("{id}/permissions")]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse>> AddPermission(
		[FromRoute] string id,
		[FromBody] AddPermissionToGroupRequest request,
		CancellationToken cancellationToken) {
		await userGroupApplication.AddPermissionAsync(id, request, cancellationToken);
		return Ok(ApiResponse.Empty());
	}

	[HttpDelete("{id}/permissions/{permissionId}")]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse>> RemovePermission(
		[FromRoute] string id,
		[FromRoute] string permissionId,
		CancellationToken cancellationToken) {
		await userGroupApplication.RemovePermissionAsync(id, permissionId, cancellationToken);
		return Ok(ApiResponse.Empty());
	}

	[HttpPost("{id}/users")]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse>> AddUser(
		[FromRoute] string id,
		[FromBody] AddUserToGroupRequest request,
		CancellationToken cancellationToken) {
		await userGroupApplication.AddUserAsync(id, request, cancellationToken);
		return Ok(ApiResponse.Empty());
	}

	[HttpDelete("{id}/users/{userId}")]
	[RequirePermission("usergroups.manage")]
	public async Task<ActionResult<ApiResponse>> RemoveUser(
		[FromRoute] string id,
		[FromRoute] string userId,
		CancellationToken cancellationToken) {
		await userGroupApplication.RemoveUserAsync(id, userId, cancellationToken);
		return Ok(ApiResponse.Empty());
	}
}
