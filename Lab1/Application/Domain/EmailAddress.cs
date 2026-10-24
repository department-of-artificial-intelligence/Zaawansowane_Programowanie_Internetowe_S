using Application.Utilities;

namespace Application.Domain;

public sealed record EmailAddress {
	private const string EmailRegexPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

	public string Value {get;}

	private EmailAddress(string value){
		Value = value;
	}

	public static Result<EmailAddress> Create(string value) => 
		Validators.MatchesRegex(value, EmailRegexPattern, "Niepoprawny format adresu email.")
		.Map(_ => new EmailAddress(value));
}
