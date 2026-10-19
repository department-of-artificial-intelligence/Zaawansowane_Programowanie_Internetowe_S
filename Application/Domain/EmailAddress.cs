using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record EmailAddress
{
    public string Value {get;}

    private EmailAddress(string value)
    {
        Value = value;
    }
    
    public static Result<EmailAddress> Create(string value) =>
        Validators.NotEmpty(value, "Adres e-mail nie może być pusty.")
            .Bind(_ => 
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                
                return regex.IsMatch(value)
                    ? Result.Success(new EmailAddress(value))
                    : Result.Failure<EmailAddress>("Niepoprawny format adresu e-mail.");
            });
}