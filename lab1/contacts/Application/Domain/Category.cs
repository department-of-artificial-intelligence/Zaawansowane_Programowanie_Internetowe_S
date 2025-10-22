namespace Contacts.Application.Domain;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Contact> Contacts { get; private set; } = new List<Contact>();

    private Category() { }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nazwa kategorii nie może być pusta", nameof(name));

        Name = name;
    }

    public void AddContact(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        if (!Contacts.Contains(contact))
        {
            Contacts.Add(contact);
        }
    }

    public void RemoveContact(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        Contacts.Remove(contact);
    }
}