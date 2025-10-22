namespace Contacts.Application.Domain;

public class FirstName
{
    public string Value { get; private set; }

    public FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Imię nie może być puste", nameof(value));

        if (value.Length < 2)
            throw new ArgumentException("Imię musi mieć co najmniej 2 znaki", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Imię nie może mieć więcej niż 50 znaków", nameof(value));


        Value = value.Trim();
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is FirstName other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();
}