namespace Contacts.Application.Domain;

public record FirstName
{
    public string Value { get; }

    public FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Imię nie może być puste.", nameof(value));
        }

        Value = value;
    }
    
    public static implicit operator string(FirstName name) => name.Value;

    public static implicit operator FirstName(string value) => new(value);
}