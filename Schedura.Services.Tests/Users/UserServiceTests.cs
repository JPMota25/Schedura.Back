using System.Linq.Expressions;
using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Users;
using Schedura.Services.Users;

namespace Schedura.Services.Tests.Users;

public class UserServiceTests {
	[Fact]
	public async Task CreateAsync_CreatesUserAndReturnsResult() {
		var repository = new InMemoryUserRepository();
		var service = new UserService(
			repository,
			new InlineValidator<CreateUserInput>(),
			new InlineValidator<UpdateUserInput>());

		var result = await service.CreateAsync(new CreateUserInput("john", "secret1", "p1"));

		Assert.Single(repository.CreatedUsers);
		Assert.Equal("john", repository.CreatedUsers[0].Username);
		Assert.NotEqual("secret1", repository.CreatedUsers[0].Password);
		Assert.StartsWith("PBKDF2$", repository.CreatedUsers[0].Password, StringComparison.Ordinal);
		Assert.Equal("john", result.Username);
		Assert.Equal("p1", result.PersonId);
	}

	[Fact]
	public async Task UpdateAsync_WhenUserDoesNotExist_ReturnsNotFound() {
		var repository = new InMemoryUserRepository();
		var service = new UserService(
			repository,
			new InlineValidator<CreateUserInput>(),
			new InlineValidator<UpdateUserInput>());

		var result = await service.UpdateAsync(new UpdateUserInput("missing", "john", "secret1", "p1"));

		Assert.False(result.Found);
		Assert.Equal(0, repository.UpdateCalls);
	}

	[Fact]
	public async Task UpdateAsync_WhenUserExists_HashesPasswordBeforePersisting() {
		var repository = new InMemoryUserRepository();
		var existingUser = new User("john", "old-password", "p1");
		repository.UserById = existingUser;

		var service = new UserService(
			repository,
			new InlineValidator<CreateUserInput>(),
			new InlineValidator<UpdateUserInput>());

		var result = await service.UpdateAsync(new UpdateUserInput(existingUser.Id, "john", "secret2", "p1"));

		Assert.True(result.Found);
		Assert.Equal(1, repository.UpdateCalls);
		Assert.NotEqual("secret2", existingUser.Password);
		Assert.StartsWith("PBKDF2$", existingUser.Password, StringComparison.Ordinal);
	}

	private sealed class InMemoryUserRepository : IUserRepository {
		public List<User> CreatedUsers { get; } = [];
		public int UpdateCalls { get; private set; }
		public User? UserById { get; set; }

		public Task<User> CreateAsync(User entity, CancellationToken cancellationToken = default) {
			CreatedUsers.Add(entity);
			return Task.FromResult(entity);
		}

		public Task UpdateAsync(User entity, CancellationToken cancellationToken = default) {
			UpdateCalls++;
			return Task.CompletedTask;
		}

		public Task DeleteAsync(User entity, CancellationToken cancellationToken = default) {
			return Task.CompletedTask;
		}

		public Task<IReadOnlyList<User>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken = default) {
			return Task.FromResult((IReadOnlyList<User>)CreatedUsers);
		}

		public Task<bool> AnyAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default) {
			return Task.FromResult(CreatedUsers.AsQueryable().Any(predicate));
		}

		public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default) {
			if (UserById is not null && UserById.Id == id) {
				return Task.FromResult<User?>(UserById);
			}

			return Task.FromResult<User?>(CreatedUsers.FirstOrDefault(x => x.Id == id));
		}

		public Task<User?> GetByIdAsNoTrackingAsync(string id, CancellationToken cancellationToken = default) {
			return GetByIdAsync(id, cancellationToken);
		}

		public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) {
			return Task.FromResult(CreatedUsers.FirstOrDefault(x => x.Username == username));
		}

		public Task<Schedura.Domain.Interfaces.Common.PagedResult<UserResult>> GetUserReportByUiFilters(
			Schedura.Domain.Interfaces.Common.PagedQuery query, CancellationToken cancellationToken = default) {
			throw new NotImplementedException();
		}
	}
}
