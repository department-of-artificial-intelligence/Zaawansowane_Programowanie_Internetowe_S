using Application.Utilities;

namespace Application.Domain;

public sealed record PhoneNumber{
    private const string PhoneRegexPattern = @"^\+?[0-9\s\-]{7,15}$";

    public string Value { get; }

    private PhoneNumber(string value){
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value) => Validators
        .MatchesRegex(value, PhoneRegexPattern, "Niepoprawny format numeru telefonu.")
        .Map(_ => new PhoneNumber(value));
}
