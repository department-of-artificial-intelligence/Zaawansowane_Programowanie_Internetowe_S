namespace Application.Utilities;
public class Result<T>
{
public class Failure(string message) : Result<T>
{
public string Message { get; } = message;
}
public class Success(T value) : Result<T>
{
public T Value { get; } = value;
}










}
