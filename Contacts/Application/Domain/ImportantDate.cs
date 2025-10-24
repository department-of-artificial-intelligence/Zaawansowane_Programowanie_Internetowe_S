namespace Contacts.Application.Domain;

public class ImportantDate
{
    public int Id { get; private set; }
    public ImportantDateInfo Info { get; private set; } = null!;
    public Contact Contact { get; private set; } = null!;
    public int ContactId { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private ImportantDate() { }

    public ImportantDate(int id, Contact contact, Category category, ImportantDateInfo info)
    {
        Id = id;
        ContactId = contact.Id;
        Contact = contact;
        CategoryId = category.Id;
        Category = category;
        Info = info;
    }
}