namespace Contacts.Application.Domain;

public class LastName
{
    public string Value { get; private set; }

    public LastName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Nazwisko nie może być puste", nameof(value));

        if (value.Length < 2)
            throw new ArgumentException("Nazwisko musi mieć co najmniej 2 znaki", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Nazwisko nie może mieć więcej niż 100 znaków", nameof(value));

        Value = value.Trim();
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is LastName other)
            return Value == other.Value;
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();
}