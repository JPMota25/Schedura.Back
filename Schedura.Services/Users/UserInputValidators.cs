using FluentValidation;
using Schedura.Domain.Entities;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Users;

namespace Schedura.Services.Users;

public class CreateUserInputValidator : AbstractValidator<CreateUserInput> {
	public CreateUserInputValidator(
		IGenericRepository<User, string> userRepository,
		IGenericRepository<Person, string> personRepository) {
		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username é obrigatório.")
			.MaximumLength(120).WithMessage("Username deve ter no máximo 120 caracteres.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password é obrigatório.")
			.Length(6, 255).WithMessage("Password deve ter entre 6 e 255 caracteres.");

		RuleFor(x => x.PersonId)
			.NotEmpty().WithMessage("PersonId é obrigatório.")
			.MustAsync(async (personId, cancellationToken) =>
				await personRepository.GetByIdAsync(personId, cancellationToken) is not null)
			.WithMessage("PersonId informado não existe.")
			.When(x => !string.IsNullOrWhiteSpace(x.PersonId));

		RuleFor(x => x.Username)
			.MustAsync(async (username, cancellationToken) =>
				!await userRepository.AnyAsync(x => x.Username == username, cancellationToken))
			.WithMessage("Já existe um usuário com esse username.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Username));
	}
}

public class UpdateUserInputValidator : AbstractValidator<UpdateUserInput> {
	public UpdateUserInputValidator(
		IGenericRepository<User, string> userRepository,
		IGenericRepository<Person, string> personRepository) {
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Id é obrigatório.");

		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username é obrigatório.")
			.MaximumLength(120).WithMessage("Username deve ter no máximo 120 caracteres.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password é obrigatório.")
			.Length(6, 255).WithMessage("Password deve ter entre 6 e 255 caracteres.");

		RuleFor(x => x.PersonId)
			.NotEmpty().WithMessage("PersonId é obrigatório.")
			.MustAsync(async (personId, cancellationToken) =>
				await personRepository.GetByIdAsNoTrackingAsync(personId, cancellationToken) is not null)
			.WithMessage("PersonId informado não existe.")
			.When(x => !string.IsNullOrWhiteSpace(x.PersonId));

		RuleFor(x => x)
			.MustAsync(async (input, cancellationToken) =>
				!await userRepository.AnyAsync(x => x.Username == input.Username && x.Id != input.Id, cancellationToken))
			.WithMessage("Já existe um usuário com esse username.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Username) && !string.IsNullOrWhiteSpace(x.Id));
	}
}
