using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Niepoprawne żądanie (np. zły JSON, brak ciała, tekst zamiast liczby).
        // W trybie Development minimal API rzuca wtedy BadHttpRequestException;
        // bez tego warunku klient dostałby 500 zamiast 400.
        if (exception is BadHttpRequestException badRequest)
        {
            logger.LogWarning(
                "Niepoprawne żądanie do {Path}: {Message}",
                httpContext.Request.Path, badRequest.Message);

            httpContext.Response.StatusCode = badRequest.StatusCode;
            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = badRequest.StatusCode,
                    Title = "Niepoprawne żądanie",
                    Detail = badRequest.Message
                },
                cancellationToken);
            return true;
        }

        logger.LogError(
            exception,
            "Wystąpił błąd podczas uruchomienia endpointu: {Path}",
            httpContext.Request.Path
        );

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Wystąpił nieoczekiwany błąd",
                Detail = environment.IsDevelopment()
                    ? exception.Message
                    : null
            },
            cancellationToken
        );

        return true;
    }
}
