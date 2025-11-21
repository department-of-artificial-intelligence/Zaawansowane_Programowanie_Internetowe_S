namespace Contacts.Application.Domain;

public record FirstName
{
    public string Value { get; }

    public FirstName(string value)
    {
        // Przeniesienie logiki walidacji z klasy Contact
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Imię nie może być puste.", nameof(value));
        }

        Value = value;
    }

    // Niejawny operator konwersji z FirstName na string
    public static implicit operator string(FirstName name) => name.Value;

    // Niejawny operator konwersji z string na FirstName (ułatwia użycie)
    public static implicit operator FirstName(string value) => new(value);
}