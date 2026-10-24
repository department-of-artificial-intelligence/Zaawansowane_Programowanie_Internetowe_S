using System.Text.RegularExpressions;

namespace Application.Utilities;

public class Validators {
	public static Result<Unit> InRange(int value, int min, int max, string errorMessage) =>
		min <= value && value <= max ? Result.Success(Unit.Instance) : Result.Failure<Unit>(errorMessage);

	public static Result<Unit> NotNull<T>(T? value, string errorMessage) =>
		value is not null ? Result.Success(Unit.Instance) : Result.Failure<Unit>(errorMessage);
		
	public static Result<Unit> NotEmpty(string name, string errorMessage) =>
		!string.IsNullOrWhiteSpace(name) ? Result.Success(Unit.Instance) : Result.Failure<Unit>(errorMessage);

	public static Result<Unit> MatchesRegex(string value, string pattern, string errorMessage) =>
        !string.IsNullOrWhiteSpace(value) && Regex.IsMatch(value, pattern) ? Result.Success(Unit.Instance) : Result.Failure<Unit>(errorMessage);
}
