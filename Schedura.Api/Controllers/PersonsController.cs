using Microsoft.AspNetCore.Mvc;
using Schedura.Api.Common;
using Schedura.Application.Contracts.Persons;

namespace Schedura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController(IPersonApplication personApplication) : ControllerBase {
	[HttpPost]
	public async Task<ActionResult<ApiResponse<PersonResponse>>> Create([FromBody] CreatePersonRequest request, CancellationToken cancellationToken) {
		var created = await personApplication.CreateAsync(request, cancellationToken);
		return Created($"/api/persons/{created.Id}", ApiResponse<PersonResponse>.Ok(created));
	}

	[HttpGet("filters")]
	public async Task<ActionResult<ApiResponse<IReadOnlyList<PersonResponse>>>> GetByFilters(
		[FromQuery] string? search,
		[FromQuery] int limit = 10,
		CancellationToken cancellationToken = default) {
		var request = new GetPersonByFiltersRequest(search, limit);
		var results = await personApplication.GetByFiltersAsync(request, cancellationToken);
		return Ok(ApiResponse<IReadOnlyList<PersonResponse>>.Ok(results));
	}
}
