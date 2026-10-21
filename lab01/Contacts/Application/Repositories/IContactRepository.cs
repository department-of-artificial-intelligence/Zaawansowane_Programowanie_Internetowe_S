using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetAll();
    Option<Contact> GetById(Guid contactId);
    void Add(Contact contact);
    void Remove(Contact contact);

    // Czy jakikolwiek adres email jest przypisany do kategorii (zadanie 4).
    bool IsCategoryInUse(Guid categoryId);
}
