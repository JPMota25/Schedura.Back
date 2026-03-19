using FluentValidation;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Permissions;

namespace Schedura.Services.Permissions;

public class CreatePermissionInputValidator : AbstractValidator<CreatePermissionInput> {
	public CreatePermissionInputValidator(IPermissionRepository permissionRepository) {
		RuleFor(x => x.Action)
			.NotEmpty().WithMessage("Action é obrigatória.")
			.MaximumLength(80).WithMessage("Action deve ter no máximo 80 caracteres.")
			.Matches(@"^[a-z]+\.[a-z]+$").WithMessage("Action deve estar no formato 'recurso.acao' (ex: users.create).")
			.MustAsync(async (action, ct) =>
				!await permissionRepository.AnyAsync(p => p.Action == action && p.DeletedAt == null, ct))
			.WithMessage("Já existe uma permissão com essa action.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Action));

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Nome é obrigatório.")
			.MaximumLength(120).WithMessage("Nome deve ter no máximo 120 caracteres.");

		RuleFor(x => x.Description)
			.MaximumLength(255).WithMessage("Descrição deve ter no máximo 255 caracteres.");
	}
}

public class UpdatePermissionInputValidator : AbstractValidator<UpdatePermissionInput> {
	public UpdatePermissionInputValidator(IPermissionRepository permissionRepository) {
		RuleFor(x => x.Id)
			.NotEmpty().WithMessage("Id é obrigatório.");

		RuleFor(x => x.Action)
			.NotEmpty().WithMessage("Action é obrigatória.")
			.MaximumLength(80).WithMessage("Action deve ter no máximo 80 caracteres.")
			.Matches(@"^[a-z]+\.[a-z]+$").WithMessage("Action deve estar no formato 'recurso.acao' (ex: users.create).")
			.MustAsync(async (input, action, ct) =>
				!await permissionRepository.AnyAsync(
					p => p.Action == action && p.Id != input.Id && p.DeletedAt == null, ct))
			.WithMessage("Já existe uma permissão com essa action.")
			.WithErrorCode("BUSINESS_CONFLICT")
			.When(x => !string.IsNullOrWhiteSpace(x.Action));

		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Nome é obrigatório.")
			.MaximumLength(120).WithMessage("Nome deve ter no máximo 120 caracteres.");

		RuleFor(x => x.Description)
			.MaximumLength(255).WithMessage("Descrição deve ter no máximo 255 caracteres.");
	}
}
