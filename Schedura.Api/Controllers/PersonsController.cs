using Microsoft.AspNetCore.Mvc;
using Schedura.Application.Contracts.Persons;

namespace Schedura.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonsController(IPersonApplication personApplication) : ControllerBase {
	[HttpPost]
	public async Task<ActionResult<PersonResponse>> Create([FromBody] CreatePersonRequest request, CancellationToken cancellationToken) {
		var created = await personApplication.CreateAsync(request, cancellationToken);
		return Created($"/api/persons/{created.Id}", created);
	}
}
