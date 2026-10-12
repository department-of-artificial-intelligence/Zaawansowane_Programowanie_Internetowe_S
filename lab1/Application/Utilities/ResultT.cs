namespace Application.Utilities;

public class result<T>
{
    public class Failure(string message) : Result<T>
    {
        public string Message { get; } = message;
    }

    public class Success (T value) : Result<T>
    {
        public T Value { get; } = value;
    }

    public R Match<R>(Func<T, R> onSuccess, Func<string, R> onFailure)
    {
        return this switch
        {
            Success success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Message),
            _ => throw new InvalidOperationException("Wrong Type")
        };
    }
}

public static class Resul
{
    
}