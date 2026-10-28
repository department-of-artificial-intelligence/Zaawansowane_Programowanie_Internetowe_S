using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetAll();

    // Zadanie 8 - kontakt jest odnajdywany tylko wśród kontaktów jego właściciela.
    Option<Contact> GetByIdAndUser(Guid contactId, Guid userId);
    void Add(Contact contact);
    void Remove(Contact contact);

    // Czy jakikolwiek adres email jest przypisany do kategorii.
    bool IsCategoryInUse(Guid categoryId);
}
