namespace Application.Utilities;

public class Result<T>
{
    public class Failure(string message) : Result<T>
    {
        public string Message { get; } = message;
    }public class Success(T value) : Result<T>
    {
        public T Value { get; } = value;
    }
    public R Match<R>(Func<T, R> onSuccess, Func<string, R> onFailure)
    {
        return this switch
        {
            Success success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Message),
            _ => throw new InvalidOperationException("Wrong type")
        };
    }
}
public static class Result
{
    public static Result<T> Success<T>(T value) =>
        new Result<T>.Success(value);
    public static Result<T> Failure<T>(string message) =>
        new Result<T>.Failure(message);
    public static Result<TResult> Map<T, TResult>(
        this Result<T> result, Func<T, TResult> map
    ) =>
        result.Match(
            value => Success(map(value)),
            Failure<TResult>);
    public static Result<TResult> Bind<T, TResult>(
        this Result<T> result, Func<T, Result<TResult>> bind
    ) =>
        result.Match(bind, Failure<TResult>);
    public static Result<T> Tap<T>(
        this Result<T> result, Action<T> action) =>
        result.Match(
            value => { action(value); return result; },
            _ => result);
    public static Result<Unit> Combine(params Result<Unit>[] results)
    {
        var errors = results
            .Select(result =>
                result.Match<string?>(_ => null, message => message))
            .Where(message => message is not null);
        var combinedMessage = string.Join(" ", errors);
        return combinedMessage.Length == 0
            ? Success(Unit.Instance)
            : Failure<Unit>(combinedMessage);
    }
}