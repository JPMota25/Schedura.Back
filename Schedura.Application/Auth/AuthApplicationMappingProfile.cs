using AutoMapper;
using Schedura.Application.Contracts.Auth;
using Schedura.Domain.Interfaces.Services.Auth;

namespace Schedura.Application.Auth;

public class AuthApplicationMappingProfile : Profile {
	public AuthApplicationMappingProfile() {
		CreateMap<LoginRequest, LoginInput>();
		CreateMap<LoginServiceResult, LoginResponse>();
		CreateMap<RefreshServiceResult, RefreshResponse>();
	}
}
