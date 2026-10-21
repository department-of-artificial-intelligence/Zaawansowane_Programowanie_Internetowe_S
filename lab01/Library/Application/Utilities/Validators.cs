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

    public static Result<Unit> MaxLength(
        string value, int maxLength, string errorMessage) =>
        value.Length <= maxLength
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>(errorMessage);

    public static Result<Unit> Matches(
        string value, Regex regex, string errorMessage) =>
        regex.IsMatch(value)
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>(errorMessage);

    public static Result<Unit> IsDefined<TEnum>(
        TEnum value, string errorMessage) where TEnum : struct, Enum =>
        Enum.IsDefined(value)
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>(errorMessage);
}
