using System.Text.RegularExpressions;
namespace Application.Utilities;
public static class Validators
{
public static Result<Unit> InRange(
int value, int min, int max, string errorMessage) =>
min <= value && value <= max
? Result.Success(Unit.Instance)
: Result.Failure<Unit>(errorMessage);
public static Result<Unit> NotNull<T>(T? value, string errorMessage) =>
value is not null
? Result.Success(Unit.Instance)
: Result.Failure<Unit>(errorMessage);
public static Result<Unit> NotEmpty(
string name, string errorMessage) =>
!string.IsNullOrWhiteSpace(name)
? Result.Success(Unit.Instance)
: Result.Failure<Unit>(errorMessage);


// nowe do zadania 1
public static Result<Unit> MaxLength(
    string? value, int max, string errorMessage) =>
    (value?.Length ?? 0) <= max
        ? Result.Success(Unit.Instance)
        : Result.Failure<Unit>(errorMessage);


// zadanie 2
public static Result<Unit> Matches(
    string value, Regex regex, string errorMessage) =>
    regex.IsMatch(value)
        ? Result.Success(Unit.Instance)
        : Result.Failure<Unit>(errorMessage);

public static Result<Unit> IsTrue(bool condition, string errorMessage) =>
    condition
        ? Result.Success(Unit.Instance)
        : Result.Failure<Unit>(errorMessage);
}