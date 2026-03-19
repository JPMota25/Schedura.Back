using Schedura.Domain.Interfaces.Common;

namespace Schedura.Domain.Interfaces.Services.Users;

public record CreateUserInput(string Username, string Password, string PersonId);

public record UpdateUserInput(string Id, string Username, string Password, string PersonId);

public record DeleteUserInput(string Id);

public record GetAllUsersParams();

public record GetUserByIdParams(string Id);

public record UserResult(string Id, string Username, string PersonId, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

public record UpdateUserResult(bool Found);

public record DeleteUserResult(bool Found);

public record GetUsersReportByUiFiltersParams(PagedQuery Query);
