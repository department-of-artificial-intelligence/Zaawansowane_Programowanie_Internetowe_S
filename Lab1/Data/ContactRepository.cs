using Application.Domain;
using Application.Repositories;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;
namespace Data;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    public void Add(Contact contact) => context.Contacts.Add(contact);

    public IEnumerable<Contact> GetByUser(Guid userId) =>
        context.Contacts.Where(contact => contact.UserId == userId);

    // Zadanie 8: filtr po właścicielu już w zapytaniu
    public Option<Contact> GetById(Guid contactId, Guid userId)
    {
        var contact = context.Contacts
            .SingleOrDefault(c => c.Id == contactId && c.UserId == userId);
        return contact is not null
            ? Option.Some(contact)
            : Option.None<Contact>();
    }

    public void Remove(Contact contact) => context.Remove(contact);

    public bool IsCategoryInUse(Guid categoryId) =>
        context.Contacts.Any(c => c.Emails.Any(e => e.CategoryId == categoryId));
}
