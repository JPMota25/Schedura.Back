using Microsoft.AspNetCore.Mvc;
using Schedura.Application.Contracts.Users;

namespace Schedura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserApplication userApplication) : ControllerBase {
	[HttpPost]
	public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken) {
		var created = await userApplication.CreateAsync(request, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken) {
		var users = await userApplication.GetAllAsync(new GetAllUsersRequest(), cancellationToken);
		return Ok(users);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<UserResponse>> GetById([FromRoute] string id, CancellationToken cancellationToken) {
		var user = await userApplication.GetByIdAsync(new GetUserByIdRequest(id), cancellationToken);
		if (user is null) {
			return NotFound();
		}

		return Ok(user);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken) {
		await userApplication.UpdateAsync(id, request, cancellationToken);
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken) {
		await userApplication.DeleteAsync(new DeleteUserRequest(id), cancellationToken);
		return NoContent();
	}
}
