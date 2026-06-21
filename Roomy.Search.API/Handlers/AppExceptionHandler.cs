using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Roomy.Search.API.Handlers
{
    public sealed class AppExceptionHandler(ILogger<AppExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            logger.LogError(
               exception,
               "Exception occurred: {Message}",
               exception.Message);

            var (status, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                System.ComponentModel.DataAnnotations.ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
                FluentValidation.ValidationException => (StatusCodes.Status400BadRequest, "Validation Error"),
                _ => (StatusCodes.Status500InternalServerError, "Server Error")
            };

            return await context.RequestServices
                .GetRequiredService<IProblemDetailsService>()
                .TryWriteAsync(new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = status,
                        Title = title,
                        Detail = exception.Message
                    }
                });
        }
    }
}
