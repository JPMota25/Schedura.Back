using AutoMapper;
using Schedura.Application.Contracts.Permissions;
using Schedura.Domain.Interfaces.Services.Permissions;

namespace Schedura.Application.Permissions;

public class PermissionApplicationMappingProfile : Profile {
	public PermissionApplicationMappingProfile() {
		CreateMap<CreatePermissionRequest, CreatePermissionInput>();
		CreateMap<PermissionResult, PermissionResponse>();
		CreateMap<UpdatePermissionResult, UpdatePermissionResponse>();
		CreateMap<DeletePermissionResult, DeletePermissionResponse>();
	}
}
