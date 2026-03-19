using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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

		var (statusCode, title, code) = exception switch {
			ValidationException validationException when IsConflictValidation(validationException) => (StatusCodes.Status409Conflict, "Business conflict", "BUSINESS_CONFLICT"),
			ValidationException => (StatusCodes.Status422UnprocessableEntity, "Business validation error", "BUSINESS_VALIDATION"),
			_ => (StatusCodes.Status500InternalServerError, "Internal server error", "UNHANDLED_ERROR")
		};

		context.Response.StatusCode = statusCode;
		context.Response.ContentType = "application/json";

		var problemDetails = new ProblemDetails {
			Status = statusCode,
			Title = title,
			Detail = exception is ValidationException fluentValidationException
				? string.Join("; ", fluentValidationException.Errors.Select(x => x.ErrorMessage).Distinct())
				: exception.Message,
			Type = $"https://httpstatuses.com/{statusCode}"
		};
		problemDetails.Extensions["traceId"] = context.TraceIdentifier;
		problemDetails.Extensions["code"] = code;
		if (exception is ValidationException ex) {
			problemDetails.Extensions["errors"] = ex.Errors.Select(x => new {
				field = x.PropertyName,
				message = x.ErrorMessage,
				code = x.ErrorCode
			});
		}

		await context.Response.WriteAsJsonAsync(problemDetails);
	}

	private static bool IsConflictValidation(ValidationException validationException) {
		return validationException.Errors.Any(x => string.Equals(x.ErrorCode, "BUSINESS_CONFLICT", StringComparison.Ordinal));
	}
}
