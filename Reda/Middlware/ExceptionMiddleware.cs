using Microsoft.AspNetCore.Mvc;
using Reda.Exceptions;
using FluentValidation;
namespace Reda.Middlware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                var codeStatus = 500;
                var message = "An unexpected error occurred";
                var title = "Internal Server Error";
                if (ex is ValidationException validationException)
                {
                    var errors = validationException.Errors.GroupBy(e => e.PropertyName).ToDictionary(e => e.Key, e => e.Select(x => x.ErrorMessage).ToArray());
                    var responseValidation = new ValidationProblemDetails(errors)
                    {
                        Title = "Validation Error",
                        Status = 400,
                        Detail = "One or more validation errors occurred.",
                        Instance = context.Request.Path
                    };
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(responseValidation);
                    return;
                }
                else if (ex is NotFoundException)
                        {
                            codeStatus = 404;
                            message = ex.Message;
                            title = "Not Found";

                        }
                else if (ex is BadRequestException)
                        {
                            codeStatus = 400;
                            message = ex.Message;
                            title = "Bad Request";
                        }
                else if (ex is UnauthorizedException)
                        {
                            codeStatus = 401;
                            message = ex.Message;
                            title = "Unauthorized";
                        }
                context.Response.StatusCode = codeStatus;
                context.Response.ContentType = "application/json";
                var response = new ProblemDetails { Title = title, Status = codeStatus, Detail = message, Instance = context.Request.Path };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}