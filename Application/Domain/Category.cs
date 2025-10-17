namespace Contacts.Application.Domain;

public class Category(string name, int id)
{
    public Category(string name)
    : this(name, 0)
    {
    }
    public int Id { get; private set; } = id;
    public string Name { get; private set; } = name;

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Nazwa kategorii nie może być pusta.", nameof(newName));
        }
        Name = newName;
    }
}