using Schedura.Domain.Enums;

namespace Schedura.Domain.Interfaces.Services.Persons;

public record CreatePersonInput(
	string Name,
	string Surname,
	string Email,
	string PhoneNumber,
	string Address,
	DateOnly BirthDate,
	string Gender,
	string Document,
	PersonType PersonType);

public record PersonResult(
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
	PersonType PersonType,
	DateTimeOffset CreatedAt,
	DateTimeOffset? UpdatedAt);
