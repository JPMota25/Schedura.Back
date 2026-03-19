using FluentValidation;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.UserGroups;

namespace Schedura.Services.UserGroups;

public class CreateUserGroupInputValidator : AbstractValidator<CreateUserGroupInput> {
	public CreateUserGroupInputValidator(IUserGroupRepository userGroupRepository) {
		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Nome é obrigatório.")
			.MaximumLength(80).WithMessage("Nome deve ter no máximo 80 caracteres.")
			.MustAsync(async (name, ct) =>
				!await userGroupRepository.AnyAsync(g => g.Name == name && g.DeletedAt == null, ct))
			.WithMessage("Já existe um grupo com esse nome.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Name));

		RuleFor(x => x.Description)
			.MaximumLength(255).WithMessage("Descrição deve ter no máximo 255 caracteres.");
	}
}

public class UpdateUserGroupInputValidator : AbstractValidator<UpdateUserGroupInput> {
	public UpdateUserGroupInputValidator(IUserGroupRepository userGroupRepository) {
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Id é obrigatório.");

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Nome é obrigatório.")
			.MaximumLength(80).WithMessage("Nome deve ter no máximo 80 caracteres.")
			.MustAsync(async (input, name, ct) =>
				!await userGroupRepository.AnyAsync(
					g => g.Name == name && g.Id != input.Id && g.DeletedAt == null, ct))
			.WithMessage("Já existe um grupo com esse nome.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Name));

		RuleFor(x => x.Description)
			.MaximumLength(255).WithMessage("Descrição deve ter no máximo 255 caracteres.");
	}
}
