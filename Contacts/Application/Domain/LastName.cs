namespace Contacts.Application.Domain;

public record LastName
{
    public string Value { get; }

    public LastName(string value)
    {
        // Przeniesienie logiki walidacji z klasy Contact
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Nazwisko nie może być puste.", nameof(value));
        }

        Value = value;
    }

    // Niejawny operator konwersji z LastName na string
    public static implicit operator string(LastName name) => name.Value;

    // Niejawny operator konwersji z string na LastName (ułatwia użycie)
    public static implicit operator LastName(string value) => new(value);
}