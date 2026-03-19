using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedura.Api.Authorization;
using Schedura.Api.Common;
using Schedura.Application.Contracts.Permissions;

namespace Schedura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController(IPermissionApplication permissionApplication) : ControllerBase {

	[HttpPost]
	[RequirePermission("permissions.manage")]
	public async Task<ActionResult<ApiResponse<PermissionResponse>>> Create(
		[FromBody] CreatePermissionRequest request,
		CancellationToken cancellationToken) {
		var response = await permissionApplication.CreateAsync(request, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = response.Id }, ApiResponse<PermissionResponse>.Ok(response));
	}

	[HttpGet]
	[RequirePermission("permissions.manage")]
	public async Task<ActionResult<ApiResponse<IReadOnlyList<PermissionResponse>>>> GetAll(CancellationToken cancellationToken) {
		var response = await permissionApplication.GetAllAsync(cancellationToken);
		return Ok(ApiResponse<IReadOnlyList<PermissionResponse>>.Ok(response));
	}

	[HttpGet("{id}")]
	[RequirePermission("permissions.manage")]
	public async Task<ActionResult<ApiResponse<PermissionResponse>>> GetById(
		[FromRoute] string id,
		CancellationToken cancellationToken) {
		var response = await permissionApplication.GetByIdAsync(id, cancellationToken);
		if (response is null) return NotFound(ApiResponse<PermissionResponse>.Fail("Permission not found."));
		return Ok(ApiResponse<PermissionResponse>.Ok(response));
	}

	[HttpPut("{id}")]
	[RequirePermission("permissions.manage")]
	public async Task<ActionResult<ApiResponse>> Update(
		[FromRoute] string id,
		[FromBody] UpdatePermissionRequest request,
		CancellationToken cancellationToken) {
		var response = await permissionApplication.UpdateAsync(id, request, cancellationToken);
		if (!response.Found) return NotFound(ApiResponse.Fail("Permission not found."));
		return Ok(ApiResponse.Empty());
	}

	[HttpDelete("{id}")]
	[RequirePermission("permissions.manage")]
	public async Task<ActionResult<ApiResponse>> Delete(
		[FromRoute] string id,
		CancellationToken cancellationToken) {
		var response = await permissionApplication.DeleteAsync(id, cancellationToken);
		if (!response.Found) return NotFound(ApiResponse.Fail("Permission not found."));
		return Ok(ApiResponse.Empty());
	}
}
