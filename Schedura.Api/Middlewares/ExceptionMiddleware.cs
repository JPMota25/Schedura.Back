using FluentValidation;
using Schedura.Api.Common;

namespace Schedura.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) {
	public async Task InvokeAsync(HttpContext context) {
		try {
			await next(context);
		}
		catch (Exception ex) {
			await HandleExceptionAsync(context, ex);
		}
	}

	private async Task HandleExceptionAsync(HttpContext context, Exception exception) {
		logger.LogError(exception, "Unhandled exception while processing request.");

		var (statusCode, title) = exception switch {
			ValidationException validationException when IsConflictValidation(validationException) => (StatusCodes.Status409Conflict, "Conflito de dados"),
			ValidationException => (StatusCodes.Status422UnprocessableEntity, "Erro de validação"),
			_ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor. Tente novamente mais tarde.")
		};

		context.Response.StatusCode = statusCode;
		context.Response.ContentType = "application/json";

		IEnumerable<string> errors = exception is ValidationException ve
			? ve.Errors.Select(e => e.ErrorMessage).Distinct()
			: [title];

		var apiResponse = ApiResponse.Fail(errors);
		await context.Response.WriteAsJsonAsync(apiResponse);
	}

	private static bool IsConflictValidation(ValidationException validationException) {
		return validationException.Errors.Any(x => string.Equals(x.ErrorCode, "BUSINESS_CONFLICT", StringComparison.Ordinal));
	}
}
