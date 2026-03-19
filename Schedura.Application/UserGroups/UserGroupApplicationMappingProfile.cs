using AutoMapper;
using Schedura.Application.Contracts.UserGroups;
using Schedura.Domain.Interfaces.Services.UserGroups;

namespace Schedura.Application.UserGroups;

public class UserGroupApplicationMappingProfile : Profile {
	public UserGroupApplicationMappingProfile() {
		CreateMap<CreateUserGroupRequest, CreateUserGroupInput>();
		CreateMap<UserGroupResult, UserGroupResponse>();
		CreateMap<UpdateUserGroupResult, UpdateUserGroupResponse>();
		CreateMap<DeleteUserGroupResult, DeleteUserGroupResponse>();
	}
}
