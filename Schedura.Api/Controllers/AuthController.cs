using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedura.Api.Common;
using Schedura.Application.Contracts.Auth;

namespace Schedura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthApplication authApplication) : ControllerBase {

	[HttpPost("login")]
	public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
		[FromBody] LoginRequest request,
		CancellationToken cancellationToken) {
		var response = await authApplication.LoginAsync(request, cancellationToken);
		return Ok(ApiResponse<LoginResponse>.Ok(response));
	}

	[HttpPost("refresh")]
	public async Task<ActionResult<ApiResponse<RefreshResponse>>> Refresh(
		[FromBody] RefreshRequest request,
		CancellationToken cancellationToken) {
		var response = await authApplication.RefreshAsync(request, cancellationToken);
		return Ok(ApiResponse<RefreshResponse>.Ok(response));
	}

	[HttpPost("logout")]
	[Authorize]
	public async Task<ActionResult<ApiResponse>> Logout(
		[FromBody] LogoutRequest request,
		CancellationToken cancellationToken) {
		await authApplication.LogoutAsync(request, cancellationToken);
		return Ok(ApiResponse.Empty());
	}
}
