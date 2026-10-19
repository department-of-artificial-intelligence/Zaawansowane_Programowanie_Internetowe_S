namespace Application.Utilities;

public static class Validators
{
  public static Result<Unit> InRange(
    int value, int min, int max, string errorMessage) =>
    min<=value && value <= max
      ? Result.Success(Unit.Instace)
      : Result.Failure<Unit>(errorMessage);

  public staitc Result<Unit> NotNull<T>(T? value, string errorMessage) =>
    value is not null
      ? Result.Success(Unit.Instance)
      : Result.Failure<Unit>(errorMessage);
  
  public static Result<unit> NotEmpty(
    string name, string errorMessage) =>
    !string.IsNullOrWhiteSpace(name)
      ?Result.Success(Unit.Instance)
      :Result.Failure<Unit>(errorMessage);
}
