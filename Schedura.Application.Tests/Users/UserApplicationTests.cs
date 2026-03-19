using AutoMapper;
using Schedura.Application.Contracts.Persons;
using Schedura.Application.Contracts.Users;
using Schedura.Application.Persons;
using Schedura.Application.Users;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Persons;
using Schedura.Domain.Interfaces.Services.Users;

namespace Schedura.Application.Tests.Users;

public class UserApplicationTests {
	private readonly IMapper mapper;

	public UserApplicationTests() {
		var mapperConfiguration = new MapperConfiguration(cfg => {
			cfg.AddProfile<UserApplicationMappingProfile>();
			cfg.AddProfile<PersonApplicationMappingProfile>();
		});
		mapper = mapperConfiguration.CreateMapper();
	}

	[Fact]
	public async Task CreateAsync_WhenServiceSucceeds_CommitsTransaction() {
		var service = new FakeUserService {
			CreateResult = new UserResult("u1", "john", "p1", DateTimeOffset.UtcNow, null)
		};
		var personService = new FakePersonService();
		var unitOfWork = new SpyUnitOfWork();
		var sut = new UserApplication(personService, service, unitOfWork, mapper);

		var result = await sut.CreateAsync(new CreateUserRequest(
			"john",
			"secret1",
			new CreatePersonRequest("Ana", "Silva", "ana@slotme.com", "11999990000", "Rua A", new DateOnly(1990, 1, 10), "F", "123.456.789-01", 0)));

		Assert.Equal("u1", result.Id);
		Assert.Equal(1, personService.CreateCalls);
		Assert.Equal(1, unitOfWork.BeginCalls);
		Assert.Equal(1, unitOfWork.CommitCalls);
		Assert.Equal(0, unitOfWork.RollbackCalls);
	}

	[Fact]
	public async Task UpdateAsync_WhenServiceThrows_RollsBackTransaction() {
		var service = new FakeUserService {
			UpdateException = new InvalidOperationException("failure")
		};
		var personService = new FakePersonService();
		var unitOfWork = new SpyUnitOfWork();
		var sut = new UserApplication(personService, service, unitOfWork, mapper);

		await Assert.ThrowsAsync<InvalidOperationException>(() =>
			sut.UpdateAsync("u1", new UpdateUserRequest("john", "secret1", "p1")));

		Assert.Equal(1, unitOfWork.BeginCalls);
		Assert.Equal(0, unitOfWork.CommitCalls);
		Assert.Equal(1, unitOfWork.RollbackCalls);
	}

	[Fact]
	public async Task GetAllAsync_DoesNotOpenTransaction() {
		var service = new FakeUserService {
			GetAllResult = [
				new UserResult("u1", "john", "p1", DateTimeOffset.UtcNow, null)
			]
		};
		var personService = new FakePersonService();
		var unitOfWork = new SpyUnitOfWork();
		var sut = new UserApplication(personService, service, unitOfWork, mapper);

		var result = await sut.GetAllAsync(new GetAllUsersRequest());

		Assert.Single(result);
		Assert.Equal(0, unitOfWork.BeginCalls);
		Assert.Equal(0, unitOfWork.CommitCalls);
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

	private sealed class FakeUserService : IUserService {
		public UserResult? CreateResult { get; init; }
		public IReadOnlyList<UserResult> GetAllResult { get; init; } = [];
		public UserResult? GetByIdResult { get; init; }
		public UpdateUserResult UpdateResult { get; init; } = new(true);
		public DeleteUserResult DeleteResult { get; init; } = new(true);
		public Exception? UpdateException { get; init; }

		public Task<UserResult> CreateAsync(CreateUserInput input, CancellationToken cancellationToken = default) {
			return Task.FromResult(CreateResult ?? new UserResult("u1", input.Username, input.PersonId, DateTimeOffset.UtcNow, null));
		}

		public Task<IReadOnlyList<UserResult>> GetAllAsync(GetAllUsersParams @params, CancellationToken cancellationToken = default) {
			return Task.FromResult(GetAllResult);
		}

		public Task<UserResult?> GetByIdAsync(GetUserByIdParams @params, CancellationToken cancellationToken = default) {
			return Task.FromResult(GetByIdResult);
		}

		public Task<UpdateUserResult> UpdateAsync(UpdateUserInput input, CancellationToken cancellationToken = default) {
			if (UpdateException is not null) {
				throw UpdateException;
			}

			return Task.FromResult(UpdateResult);
		}

		public Task<DeleteUserResult> DeleteAsync(DeleteUserInput input, CancellationToken cancellationToken = default) {
			return Task.FromResult(DeleteResult);
		}

		public Task<Schedura.Domain.Interfaces.Common.PagedResult<UserResult>> SearchAsync(
			SearchUsersParams @params, CancellationToken cancellationToken = default) {
			throw new NotImplementedException();
		}
	}

	private sealed class FakePersonService : IPersonService {
		public int CreateCalls { get; private set; }

		public Task<PersonResult> CreateAsync(CreatePersonInput input, CancellationToken cancellationToken = default) {
			CreateCalls++;
			return Task.FromResult(new PersonResult(
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

		public Task<IReadOnlyList<PersonResult>> GetByFiltersAsync(GetPersonByFiltersParams @params, CancellationToken cancellationToken = default) {
			return Task.FromResult<IReadOnlyList<PersonResult>>([]);
		}
	}
}
