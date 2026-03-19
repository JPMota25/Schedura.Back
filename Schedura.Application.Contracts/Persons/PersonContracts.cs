using System.ComponentModel.DataAnnotations;

namespace Schedura.Application.Contracts.Persons;

public record CreatePersonRequest(
	[property: Required(AllowEmptyStrings = false), StringLength(120)] string Name,
	[property: Required(AllowEmptyStrings = false), StringLength(120)] string Surname,
	[property: Required(AllowEmptyStrings = false), StringLength(180), EmailAddress] string Email,
	[property: Required(AllowEmptyStrings = false), StringLength(20)] string PhoneNumber,
	[property: Required(AllowEmptyStrings = false), StringLength(250)] string Address,
	[property: Required] DateOnly BirthDate,
	[property: Required(AllowEmptyStrings = false), StringLength(30)] string Gender,
	[property: Required(AllowEmptyStrings = false), StringLength(18)] string Document,
	[property: Range(0, 1)] int PersonType);

public record PersonResponse(
	string Id,
	string Name,
	string Surname,
	string Email,
	string PhoneNumber,
	string Address,
	DateOnly BirthDate,
	string Gender,
	string Document,
	string FormattedDocument,
	int PersonType,
	DateTimeOffset CreatedAt,
	DateTimeOffset? UpdatedAt);

public record GetPersonByFiltersRequest(string? Search, int Limit = 10);
