using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Enums;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Persons;

namespace Schedura.Services.Persons;

public class CreatePersonInputValidator : AbstractValidator<CreatePersonInput> {
	public CreatePersonInputValidator(IGenericRepository<Person, string> personRepository) {
		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Name é obrigatório.")
			.MaximumLength(120).WithMessage("Name deve ter no máximo 120 caracteres.");

		RuleFor(x => x.Surname)
			.NotEmpty().WithMessage("Surname é obrigatório.")
			.MaximumLength(120).WithMessage("Surname deve ter no máximo 120 caracteres.");

		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email é obrigatório.")
			.MaximumLength(180).WithMessage("Email deve ter no máximo 180 caracteres.")
			.EmailAddress().WithMessage("Email inválido.")
			.MustAsync(async (email, cancellationToken) =>
				!await personRepository.AnyAsync(x => x.Email == email, cancellationToken))
			.WithMessage("Já existe uma pessoa com esse email.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Email));

		RuleFor(x => x.PhoneNumber)
			.NotEmpty().WithMessage("PhoneNumber é obrigatório.")
			.MaximumLength(20).WithMessage("PhoneNumber deve ter no máximo 20 caracteres.");

		RuleFor(x => x.Address)
			.NotEmpty().WithMessage("Address é obrigatório.")
			.MaximumLength(250).WithMessage("Address deve ter no máximo 250 caracteres.");

		RuleFor(x => x.BirthDate)
			.NotEqual(default(DateOnly)).WithMessage("BirthDate é obrigatório.")
			.LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("BirthDate não pode ser futura.");

		RuleFor(x => x.Gender)
			.NotEmpty().WithMessage("Gender é obrigatório.")
			.MaximumLength(30).WithMessage("Gender deve ter no máximo 30 caracteres.");

		RuleFor(x => x.Document)
			.NotEmpty().WithMessage("Document é obrigatório.")
			.MaximumLength(18).WithMessage("Document deve ter no máximo 18 caracteres.")
			.Must((input, document) => HasValidDocumentLength(document, input.PersonType))
			.WithMessage("Document inválido para o tipo de pessoa informado.")
			.MustAsync(async (_, document, cancellationToken) => {
				var normalizedDocument = NormalizeDocument(document);
				return !await personRepository.AnyAsync(x => x.Document == normalizedDocument, cancellationToken);
			})
			.WithMessage("Já existe uma pessoa com esse documento.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Document));
	}

	private static bool HasValidDocumentLength(string document, PersonType personType) {
		var digitsOnly = NormalizeDocument(document);
		return personType switch {
			PersonType.Individual => digitsOnly.Length == 11,
			PersonType.Company => digitsOnly.Length == 14,
			_ => false
		};
	}

	private static string NormalizeDocument(string document) {
		return new string(document.Where(char.IsDigit).ToArray());
	}
}
