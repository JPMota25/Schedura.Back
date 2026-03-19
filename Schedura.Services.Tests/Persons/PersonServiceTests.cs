using System.Linq.Expressions;
using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Enums;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Persons;
using Schedura.Services.Persons;

namespace Schedura.Services.Tests.Persons;

public class PersonServiceTests {
	[Fact]
	public async Task CreateAsync_NormalizesDocumentAndReturnsFormattedDocument() {
		var repository = new InMemoryPersonRepository();
		var service = new PersonService(repository, new InlineValidator<CreatePersonInput>());

		var result = await service.CreateAsync(new CreatePersonInput(
			"Ana",
			"Silva",
			"ana@slotme.com",
			"11999990000",
			"Rua A",
			new DateOnly(1990, 1, 10),
			"F",
			"123.456.789-01",
			PersonType.Individual));

		Assert.Single(repository.CreatedPersons);
		Assert.Equal("12345678901", repository.CreatedPersons[0].Document);
		Assert.Equal("12345678901", result.Document);
		Assert.Equal("123.456.789-01", result.FormattedDocument);
	}

	private sealed class InMemoryPersonRepository : IPersonRepository {
		public List<Person> CreatedPersons { get; } = [];

		public Task<Person> CreateAsync(Person entity, CancellationToken cancellationToken = default) {
			CreatedPersons.Add(entity);
			return Task.FromResult(entity);
		}

		public Task UpdateAsync(Person entity, CancellationToken cancellationToken = default) {
			return Task.CompletedTask;
		}

		public Task DeleteAsync(Person entity, CancellationToken cancellationToken = default) {
			return Task.CompletedTask;
		}

		public Task<IReadOnlyList<Person>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken = default) {
			return Task.FromResult((IReadOnlyList<Person>)CreatedPersons);
		}

		public Task<bool> AnyAsync(Expression<Func<Person, bool>> predicate, CancellationToken cancellationToken = default) {
			return Task.FromResult(CreatedPersons.AsQueryable().Any(predicate));
		}

		public Task<Person?> GetByIdAsync(string id, CancellationToken cancellationToken = default) {
			return Task.FromResult<Person?>(CreatedPersons.FirstOrDefault(x => x.Id == id));
		}

		public Task<Person?> GetByIdAsNoTrackingAsync(string id, CancellationToken cancellationToken = default) {
			return GetByIdAsync(id, cancellationToken);
		}

		public Task<IReadOnlyList<Person>> GetByFiltersAsync(string? search, int limit, CancellationToken cancellationToken = default) {
			return Task.FromResult((IReadOnlyList<Person>)CreatedPersons);
		}
	}
}
