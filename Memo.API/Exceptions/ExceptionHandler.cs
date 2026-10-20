using FluentValidation;
using Memo.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Memo.API.Exceptions;

internal sealed class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException => CreateValidationProblem(validationException),
            DomainException => Create(StatusCodes.Status422UnprocessableEntity, "Business Rule Violation", exception.Message),
            DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } } =>
                Create(StatusCodes.Status409Conflict, "Conflict", "A record with the same unique value already exists."),
            _ when IsEntityNotFound(exception) => Create(StatusCodes.Status404NotFound, "Not Found", exception.Message),
            _ => null
        };

        if (problemDetails is null)
        {
            logger.LogError(exception, "Unhandled exception");
            problemDetails = Create(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
        else
        {
            logger.LogWarning("Request failed ({Status}): {Message}", problemDetails.Status, exception.Message);
        }

        problemDetails.Instance = httpContext.Request.Path;
        httpContext.Response.StatusCode = problemDetails.Status!.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static bool IsEntityNotFound(Exception exception) =>
        exception.GetType() is { IsGenericType: true } type &&
        type.GetGenericTypeDefinition() == typeof(EntityNotFoundException<>);

    private static ProblemDetails Create(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };

    private static ProblemDetails CreateValidationProblem(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
        var problemDetails = Create(
            StatusCodes.Status400BadRequest,
            "Request Failed",
            "One or more validation errors occurred.");
        problemDetails.Extensions["errors"] = errors;
        return problemDetails;
    }
}
