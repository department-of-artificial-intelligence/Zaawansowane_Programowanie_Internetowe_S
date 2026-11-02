using Application.Domain;
using Application.Utilities;
namespace Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetByUser(Guid userId);

    // Zadanie 8: kontakt jest zwracany tylko swojemu właścicielowi
    Option<Contact> GetById(Guid contactId, Guid userId);

    void Add(Contact contact);
    void Remove(Contact contact);
    bool IsCategoryInUse(Guid categoryId);
}
