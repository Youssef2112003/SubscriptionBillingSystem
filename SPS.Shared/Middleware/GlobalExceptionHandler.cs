using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SPS.Shared.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = CreateProblemDetails(httpContext, exception);

        LogException(exception, problemDetails, httpContext);

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            System.ComponentModel.DataAnnotations.ValidationException or FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(exception),
            Detail = GetDetail(exception),
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        if (exception is FluentValidation.ValidationException fluentEx)
        {
            AddValidationErrors(problemDetails, fluentEx.Errors);
        }
        else if (exception is System.ComponentModel.DataAnnotations.ValidationException validationEx && validationEx.ValidationResult != null)
        {

            var errors = validationEx.ValidationResult.ErrorMessage != null
                ? new[] { new { PropertyName = string.Empty, ErrorMessage = validationEx.ValidationResult.ErrorMessage } }
                : validationEx.ValidationResult.MemberNames.Select(m => new { PropertyName = m, ErrorMessage = $"Validation failed for {m}" });
            AddValidationErrors(problemDetails, errors);
        }


        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.ToString();
        }

        return problemDetails;
    }

    private static string GetTitle(Exception exception) => exception switch
    {
        FluentValidation.ValidationException => "Validation Error",
        UnauthorizedAccessException => "Unauthorized",
        KeyNotFoundException => "Resource Not Found",
        InvalidOperationException => "Invalid Operation",
        _ => "Server Error"
    };

    private string GetDetail(Exception exception) =>
        _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred. Please try again later.";

    private static void AddValidationErrors(ProblemDetails problemDetails, IEnumerable<object> errors)
    {
        problemDetails.Extensions["errors"] = errors;
    }

    private void LogException(Exception exception, ProblemDetails problemDetails, HttpContext context)
    {
        var logLevel = exception switch
        {
            FluentValidation.ValidationException => LogLevel.Warning,
            UnauthorizedAccessException => LogLevel.Warning,
            KeyNotFoundException => LogLevel.Information,
            _ => LogLevel.Error
        };

        _logger.Log(logLevel, exception,
            "Request {Method} {Path} failed with {StatusCode}. TraceId: {TraceId}",
            context.Request.Method, context.Request.Path, problemDetails.Status, context.TraceIdentifier);
    }
}