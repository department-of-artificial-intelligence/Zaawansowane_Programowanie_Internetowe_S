namespace Application.Utilities;

public class Option<T>
{
    public class None() : Option<T>
    {
    }
    public class Some(T value) : Option<T>
    {
        public T Value { get; } = value;
    }
    public R Match<R>(Func<T, R> onSuccess, Func<R> onFailure)
    {
        return this switch
        {
            Some some => onSuccess(some.Value),
            None _ => onFailure(),
            _ => throw new InvalidOperationException("Wrong type")
        };
    }
}

public static class Option
{
    public static Option<T> Some<T>(T value) =>
    new Option<T>.Some(value);
    public static Option<T> None<T>() =>
    new Option<T>.None();
    public static Result<T> Required<T>(
    this Option<T> option, string noneMessage)
    => option switch
    {
        Option<T>.Some some => Result.Success(some.Value),
        Option<T>.None _ => Result.Failure<T>(noneMessage),
        _ => throw new InvalidOperationException()
    };
}