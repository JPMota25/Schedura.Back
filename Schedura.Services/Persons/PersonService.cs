using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Persons;

namespace Schedura.Services.Persons;

public class PersonService(
	IGenericRepository<Person, string> personRepository,
	IValidator<CreatePersonInput> createPersonValidator) : IPersonService {
	public async Task<PersonResult> CreateAsync(CreatePersonInput input, CancellationToken cancellationToken = default) {
		await createPersonValidator.ValidateAndThrowAsync(input, cancellationToken);

		var normalizedDocument = NormalizeDocument(input.Document);
		var person = new Person(
			input.Name,
			input.Surname,
			input.Email,
			input.PhoneNumber,
			input.Address,
			input.BirthDate,
			input.Gender,
			normalizedDocument,
			input.PersonType);

		await personRepository.CreateAsync(person, cancellationToken);
		return ToResult(person);
	}

	private static PersonResult ToResult(Person person) {
		return new PersonResult(
			person.Id,
			person.Name,
			person.Surname,
			person.Email,
			person.PhoneNumber,
			person.Address,
			person.BirthDate,
			person.Gender,
			person.Document,
			person.FormattedDocument,
			person.PersonType,
			person.CreatedAt,
			person.UpdatedAt);
	}

	private static string NormalizeDocument(string document) {
		return new string(document.Where(char.IsDigit).ToArray());
	}
}
