using AutoMapper;
using Schedura.Application.Contracts.Persons;
using Schedura.Application.Persons;
using Schedura.Domain.Enums;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Persons;

namespace Schedura.Application.Tests.Persons;

public class PersonApplicationTests {
	private readonly IMapper mapper;

	public PersonApplicationTests() {
		var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<PersonApplicationMappingProfile>());
		mapper = mapperConfiguration.CreateMapper();
	}

	[Fact]
	public async Task CreateAsync_WhenServiceSucceeds_CommitsTransaction() {
		var service = new FakePersonService {
			CreateResult = new PersonResult(
				"p1",
				"Ana",
				"Silva",
				"ana@slotme.com",
				"11999990000",
				"Rua A",
				new DateOnly(1990, 1, 10),
				"F",
				"12345678901",
				"123.456.789-01",
				PersonType.Individual,
				DateTimeOffset.UtcNow,
				null)
		};
		var unitOfWork = new SpyUnitOfWork();
		var sut = new PersonApplication(service, unitOfWork, mapper);

		var result = await sut.CreateAsync(new CreatePersonRequest(
			"Ana",
			"Silva",
			"ana@slotme.com",
			"11999990000",
			"Rua A",
			new DateOnly(1990, 1, 10),
			"F",
			"123.456.789-01",
			0));

		Assert.Equal("p1", result.Id);
		Assert.Equal("123.456.789-01", result.FormattedDocument);
		Assert.Equal(1, unitOfWork.BeginCalls);
		Assert.Equal(1, unitOfWork.CommitCalls);
		Assert.Equal(0, unitOfWork.RollbackCalls);
	}

	private sealed class SpyUnitOfWork : IUnitOfWork {
		public int BeginCalls { get; private set; }
		public int CommitCalls { get; private set; }
		public int RollbackCalls { get; private set; }

		public Task BeginTransactionAsync(CancellationToken cancellationToken = default) {
			BeginCalls++;
			return Task.CompletedTask;
		}

		public Task CommitAsync(CancellationToken cancellationToken = default) {
			CommitCalls++;
			return Task.CompletedTask;
		}

		public Task RollbackAsync(CancellationToken cancellationToken = default) {
			RollbackCalls++;
			return Task.CompletedTask;
		}
	}

	private sealed class FakePersonService : IPersonService {
		public PersonResult? CreateResult { get; init; }

		public Task<PersonResult> CreateAsync(CreatePersonInput input, CancellationToken cancellationToken = default) {
			return Task.FromResult(CreateResult ?? new PersonResult(
				"p1",
				input.Name,
				input.Surname,
				input.Email,
				input.PhoneNumber,
				input.Address,
				input.BirthDate,
				input.Gender,
				input.Document,
				input.Document,
				input.PersonType,
				DateTimeOffset.UtcNow,
				null));
		}
	}
}
