using System.ComponentModel.DataAnnotations;
using Schedura.Application.Contracts.Persons;

namespace Schedura.Application.Contracts.Users;

public record GetAllUsersRequest();

public record GetUserByIdRequest(
	[property: Required(AllowEmptyStrings = false)] string Id);

public record CreateUserRequest(
	[property: Required(AllowEmptyStrings = false), StringLength(120)] string Username,
	[property: Required(AllowEmptyStrings = false), MinLength(6), StringLength(255)] string Password,
	[property: Required] CreatePersonRequest Person);

public record UpdateUserRequest(
	[property: Required(AllowEmptyStrings = false), StringLength(120)] string Username,
	[property: Required(AllowEmptyStrings = false), MinLength(6), StringLength(255)] string Password,
	[property: Required(AllowEmptyStrings = false)] string PersonId);

public record DeleteUserRequest(
	[property: Required(AllowEmptyStrings = false)] string Id);

public record UserResponse(string Id, string Username, string PersonId, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt);

public record UpdateUserResponse(bool Found);

public record DeleteUserResponse(bool Found);
