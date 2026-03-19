using AutoMapper;
using Schedura.Application.Contracts.Users;
using Schedura.Domain.Interfaces.Services.Users;

namespace Schedura.Application.Users;

public class UserApplicationMappingProfile : Profile {
	public UserApplicationMappingProfile() {
		CreateMap<CreateUserRequest, CreateUserInput>()
			.ConstructUsing((src, context) => new CreateUserInput(src.Username, src.Password, (string)context.Items["personId"]));

		CreateMap<GetAllUsersRequest, GetAllUsersParams>();
		CreateMap<GetUserByIdRequest, GetUserByIdParams>();
		CreateMap<DeleteUserRequest, DeleteUserInput>();
		CreateMap<UpdateUserRequest, UpdateUserInput>()
			.ConstructUsing((src, context) => new UpdateUserInput((string)context.Items["id"], src.Username, src.Password, src.PersonId));

		CreateMap<UserResult, UserResponse>();
		CreateMap<UpdateUserResult, UpdateUserResponse>();
		CreateMap<DeleteUserResult, DeleteUserResponse>();
	}
}
