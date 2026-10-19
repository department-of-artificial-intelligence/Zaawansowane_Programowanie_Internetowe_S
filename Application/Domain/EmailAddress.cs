using Application.Utilities;
namespace Application.Domain;

public sealed record EmailAddress
{
    public string Value { get; }
    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string value) =>
        Result.Success(new EmailAddress(value));


}
