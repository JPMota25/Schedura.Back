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
			ValidationException validationException when IsConflictValidation(validationException) => (StatusCodes.Status409Conflict, "Business conflict"),
			ValidationException => (StatusCodes.Status422UnprocessableEntity, "Business validation error"),
			_ => (StatusCodes.Status500InternalServerError, "Internal server error")
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
