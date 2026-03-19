using AutoMapper;
using Schedura.Application.Contracts.Auth;
using Schedura.Domain.Interfaces.Repositories;
using Schedura.Domain.Interfaces.Services.Auth;

namespace Schedura.Application.Auth;

public class AuthApplication(
	IAuthService authService,
	IUnitOfWork unitOfWork,
	IMapper mapper) : IAuthApplication {

	public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default) {
		await unitOfWork.BeginTransactionAsync(cancellationToken);
		try {
			var input = mapper.Map<LoginInput>(request);
			var result = await authService.LoginAsync(input, cancellationToken);
			await unitOfWork.CommitAsync(cancellationToken);
			return mapper.Map<LoginResponse>(result);
		}
		catch {
			await unitOfWork.RollbackAsync(cancellationToken);
			throw;
		}
	}

	public async Task<RefreshResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken = default) {
		await unitOfWork.BeginTransactionAsync(cancellationToken);
		try {
			var result = await authService.RefreshAsync(request.RefreshToken, cancellationToken);
			await unitOfWork.CommitAsync(cancellationToken);
			return mapper.Map<RefreshResponse>(result);
		}
		catch {
			await unitOfWork.RollbackAsync(cancellationToken);
			throw;
		}
	}

	public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default) {
		await unitOfWork.BeginTransactionAsync(cancellationToken);
		try {
			await authService.LogoutAsync(request.RefreshToken, cancellationToken);
			await unitOfWork.CommitAsync(cancellationToken);
		}
		catch {
			await unitOfWork.RollbackAsync(cancellationToken);
			throw;
		}
	}
}
