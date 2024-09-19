using EventsWebApplication.Application.DTOs;

namespace EventsWebApplication.API.Middlewares;

using System.Net;
using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            switch (ex)
            {
                case ApplicationException e:
                    await HandleExceptionAsync(context, e.Message, HttpStatusCode.BadRequest, "Bad Request");
                    break;
                case KeyNotFoundException e:
                    await HandleExceptionAsync(context, e.Message, HttpStatusCode.NotFound, "Not Found");
                    break;
                case UnauthorizedAccessException e:
                    await HandleExceptionAsync(context, e.Message, HttpStatusCode.Unauthorized, "Unauthorized");
                    break;
                case InvalidOperationException e:
                    await HandleExceptionAsync(context, e.Message, HttpStatusCode.Conflict, "Conflict");
                    break;
                case ArgumentException e:
                    await HandleExceptionAsync(context, e.Message, HttpStatusCode.BadRequest, "Bad Request");
                    break;
                case FormatException e:
                    await HandleExceptionAsync(context, e.Message, HttpStatusCode.BadRequest, "Bad Request");
                    break;
                default:
                    await HandleExceptionAsync(context, ex.Message, HttpStatusCode.InternalServerError, "Internal Server Error");
                    break;
            }
        }
    }

    private Task HandleExceptionAsync(
        HttpContext context,
        string exceptionMessage,
        HttpStatusCode httpStatusCode,
        string message)
    {
        _logger.LogError(exceptionMessage);
        HttpResponse response = context.Response;
        
        response.ContentType = "application/json";
        response.StatusCode = (int)httpStatusCode;

        ErrorDto errorDto = new ErrorDto()
        {
            Message = message,
            StatusCode = (int)httpStatusCode
        };
        
        string result = JsonSerializer.Serialize(errorDto);
        
        return context.Response.WriteAsJsonAsync(result);
    }
}

