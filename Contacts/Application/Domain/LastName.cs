namespace Contacts.Application.Domain;

public record LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Nazwisko nie może być puste.", nameof(value));
        }

        Value = value;
    }

    public static implicit operator string(LastName name) => name.Value;

    public static implicit operator LastName(string value) => new(value);
}