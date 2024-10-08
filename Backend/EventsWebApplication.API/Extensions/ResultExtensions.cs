using EventsWebApplication.Application.Errors.Base;
using FluentResults;

namespace EventsWebApplication.API.Extensions;

public static class ResultExtensions
{
    public static IResult TryGetResult<TValue>(this Result<TValue> result, Func<TValue, IResult> success)
    {
        return result switch
        {
            { IsFailed: true } => result.ToProblemDetail(),
            { IsSuccess: true } => success(result.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    public static IResult TryGetResult(this Result result, Func<IResult> success)
    {
        return result switch
        {
            { IsFailed: true } => result.ToProblemDetail(),
            { IsSuccess: true } => success(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static IResult ToProblemDetail(this ResultBase result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a problem because the result is successful");
        }

        var error = result.Errors.FirstOrDefault()!;
        Dictionary<string, object?> extensions = new Dictionary<string, object?> { { "message", error.Message } };

        if (error is BaseError baseError)
        {
            extensions.Add("code", baseError.Code);
        }
        

        return Results.Problem(
            statusCode: GetStatusCode(error),
            title: GetTitle(error),
            extensions: extensions);
    }

    private static int GetStatusCode(IError error) => error switch
    {
        UnauthorizedError => StatusCodes.Status401Unauthorized,
        NotFoundError => StatusCodes.Status404NotFound,
        ConflictError => StatusCodes.Status409Conflict,
        InternalServerError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetTitle(IError error) => error switch
    {
        UnauthorizedError => "Unauthorized",
        NotFoundError => "Not Found",
        ConflictError => "Conflict",
        InternalServerError => "Internal Server Error",
        _ => "Internal server error"
    };
    
}