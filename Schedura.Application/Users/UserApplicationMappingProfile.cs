using AutoMapper;
using Schedura.Application.Contracts.Common;
using Schedura.Application.Contracts.Users;
using Schedura.Domain.Interfaces.Services.Users;
using DomainCommon = Schedura.Domain.Interfaces.Common;

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

		CreateMap<PagedRequest, DomainCommon.PagedQuery>()
			.ConstructUsing((src, _) => new DomainCommon.PagedQuery(
				src.Page, src.PageSize,
				src.Sort.Select(s => new DomainCommon.SortDescriptor(s.Field, s.Sort == "desc")).ToList(),
				src.Filters.Select(f => new DomainCommon.FilterDescriptor(f.Field, f.Operator, f.Value)).ToList(),
				(DomainCommon.FilterLogicOperator)(int)src.LogicOperator))
			.ForAllMembers(opt => opt.Ignore());

		CreateMap<GetUsersReportByUiFiltersRequest, GetUsersReportByUiFiltersParams>()
			.ConstructUsing((src, ctx) => new GetUsersReportByUiFiltersParams(ctx.Mapper.Map<DomainCommon.PagedQuery>(src.Paged)));
	}
}
