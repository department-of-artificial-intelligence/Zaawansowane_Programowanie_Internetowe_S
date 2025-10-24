namespace Contacts.Application.Domain;

public class Phone
{
    public int Id { get; private set; }
    public PhoneNumber Number { get; private set; } = null!;
    public Contact Contact { get; private set; } = null!;
    public int ContactId { get; private set; }
    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    private Phone() { }

    public Phone(int id, Contact contact, Category category, PhoneNumber number)
    {
        Id = id;
        ContactId = contact.Id;
        Contact = contact;
        CategoryId = category.Id;
        Category = category;
        Number = number;
    }
}