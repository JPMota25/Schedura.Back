using Microsoft.AspNetCore.Mvc;
using Schedura.Api.Common;
using Schedura.Application.Contracts.Users;

namespace Schedura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserApplication userApplication) : ControllerBase {
	[HttpPost]
	public async Task<ActionResult<ApiResponse<UserResponse>>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken) {
		var created = await userApplication.CreateAsync(request, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<UserResponse>.Ok(created));
	}

	[HttpGet]
	public async Task<ActionResult<ApiResponse<IReadOnlyList<UserResponse>>>> GetAll(CancellationToken cancellationToken) {
		var users = await userApplication.GetAllAsync(new GetAllUsersRequest(), cancellationToken);
		return Ok(ApiResponse<IReadOnlyList<UserResponse>>.Ok(users));
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<ApiResponse<UserResponse>>> GetById([FromRoute] string id, CancellationToken cancellationToken) {
		var user = await userApplication.GetByIdAsync(new GetUserByIdRequest(id), cancellationToken);
		if (user is null) {
			return NotFound(ApiResponse<UserResponse>.Fail("User not found."));
		}

		return Ok(ApiResponse<UserResponse>.Ok(user));
	}

	[HttpPut("{id}")]
	public async Task<ActionResult<ApiResponse>> Update([FromRoute] string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken) {
		await userApplication.UpdateAsync(id, request, cancellationToken);
		return Ok(ApiResponse.Empty());
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult<ApiResponse>> Delete([FromRoute] string id, CancellationToken cancellationToken) {
		await userApplication.DeleteAsync(new DeleteUserRequest(id), cancellationToken);
		return Ok(ApiResponse.Empty());
	}

	[HttpPost("search")]
	public async Task<ActionResult<ApiResponse<SearchUsersResponse>>> Search(
		[FromBody] SearchUsersRequest request,
		CancellationToken cancellationToken) {
		var result = await userApplication.SearchAsync(request, cancellationToken);
		return Ok(ApiResponse<SearchUsersResponse>.Ok(result));
	}
}
