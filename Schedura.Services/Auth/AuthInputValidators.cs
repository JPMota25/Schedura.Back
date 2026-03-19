using FluentValidation;
using Schedura.Domain.Interfaces.Services.Auth;

namespace Schedura.Services.Auth;

public class LoginInputValidator : AbstractValidator<LoginInput> {
	public LoginInputValidator() {
		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username é obrigatório.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password é obrigatório.");
	}
}
