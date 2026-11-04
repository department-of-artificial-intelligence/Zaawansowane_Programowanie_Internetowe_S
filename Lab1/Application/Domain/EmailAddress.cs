using Application.Utilities;
using System.Text.RegularExpressions;
namespace Application.Domain;
public sealed record EmailAddress
{
private static readonly Regex Format =
new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
public string Value {get;}
private EmailAddress(string value)
{
Value = value;
}
public static Result<EmailAddress> Create(string value) =>
!string.IsNullOrWhiteSpace(value) && Format.IsMatch(value.Trim())
? Result.Success(new EmailAddress(value.Trim()))
: Result.Failure<EmailAddress>("Adres email ma niepoprawny format.");
}
