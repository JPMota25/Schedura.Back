using AutoMapper;
using Schedura.Application.Contracts.Persons;
using Schedura.Domain.Enums;
using Schedura.Domain.Interfaces.Services.Persons;

namespace Schedura.Application.Persons;

public class PersonApplicationMappingProfile : Profile {
	public PersonApplicationMappingProfile() {
		CreateMap<CreatePersonRequest, CreatePersonInput>()
			.ConstructUsing(src => new CreatePersonInput(
				src.Name,
				src.Surname,
				src.Email,
				src.PhoneNumber,
				src.Address,
				src.BirthDate,
				src.Gender,
				src.Document,
				(PersonType)src.PersonType));

		CreateMap<PersonResult, PersonResponse>()
			.ForCtorParam(nameof(PersonResponse.PersonType), opt => opt.MapFrom(src => (int)src.PersonType));
	}
}
