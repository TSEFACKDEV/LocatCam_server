namespace LocatCam.Api.Middleware;

using LocatCam.Core.Dtos;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

/// <summary>
/// Middleware de gestion globale des exceptions.
/// Retourne des réponses d'erreur standardisées au format { status, message, errors[] }.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    /// <summary>Constructeur du middleware.</summary>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    /// <summary>Intercepte les exceptions et génère une réponse d'erreur standardisée.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur non gérée : {Message}", ex.Message);

            context.Response.ContentType = "application/problem+json";

            int statusCode = ex switch
            {
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new ApiErrorResponse
            {
                Status = statusCode,
                Message = _env.IsDevelopment() ? ex.Message : "Une erreur interne est survenue."
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json);
        }
    }
}

/// <summary>
/// Extensions d'enregistrement du middleware d'exception.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>Ajoute le middleware d'exception au pipeline.</summary>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
