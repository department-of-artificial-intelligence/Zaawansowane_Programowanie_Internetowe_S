using Application.Utilities;

namespace WebAPI;

public static class HttpResultExtensions
{
    public static IResult ToNoContent(this Result<Unit> result) =>
        result.Match(
            _ => Results.NoContent(),
            error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest));
}