using Application.Utilities;
namespace WebAPI;

/// <summary>
/// Zamiana Result&lt;T&gt; z warstwy aplikacji na odpowiedź HTTP.
/// Sukces -> to, co zwróci onSuccess; błąd -> ProblemDetails z kodem zależnym od ErrorKind.
/// </summary>
public static class ResultHttpExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess) =>
        result.Match(onSuccess, (message, kind) => ToProblem(message, kind));

    public static IResult ToNoContent<T>(this Result<T> result) =>
        result.ToHttpResult(_ => Results.NoContent());

    public static IResult NotFound(string message) =>
        ToProblem(message, ErrorKind.NotFound);

    private static IResult ToProblem(string message, ErrorKind kind) => kind switch
    {
        ErrorKind.NotFound => Results.Problem(
            title: "Nie znaleziono", detail: message,
            statusCode: StatusCodes.Status404NotFound),
        ErrorKind.Conflict => Results.Problem(
            title: "Konflikt", detail: message,
            statusCode: StatusCodes.Status409Conflict),
        _ => Results.Problem(
            title: "Niepoprawne dane", detail: message,
            statusCode: StatusCodes.Status400BadRequest)
    };
}
