using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            if (!context.Response.HasStarted)
            {
                await WriteProblemDetailsAsync(context, ex);
            }
        }
    }

    private static async Task WriteProblemDetailsAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/problem+json";

        ProblemDetails problemDetails;

        switch (ex)
        {
            case ValidationException validationException:
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var validationProblem = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed."
                };

                problemDetails = validationProblem;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case KeyNotFoundException:
                problemDetails = CreateProblemDetails(StatusCodes.Status404NotFound, "Resource not found.", ex.Message);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                break;

            case UnauthorizedAccessException:
                problemDetails = CreateProblemDetails(StatusCodes.Status401Unauthorized, "Unauthorized.", ex.Message);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                break;

            case ArgumentException:
                problemDetails = CreateProblemDetails(StatusCodes.Status400BadRequest, "Invalid request.", ex.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case InvalidOperationException:
                problemDetails = CreateProblemDetails(StatusCodes.Status409Conflict, "Invalid operation.", ex.Message);
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                break;

            default:
                problemDetails = CreateProblemDetails(StatusCodes.Status500InternalServerError, "An unexpected error occurred.", "Please try again later.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
    }

    private static ProblemDetails CreateProblemDetails(int statusCode, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}"
        };
    }
}
