using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

// Zadanie 8 - każda operacja dotyczy wyłącznie kontaktów zalogowanego użytkownika.
public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);
    Result<Unit> UpdateContact(UpdateContactDto updateContactDto);
    Result<Unit> RemoveContact(Guid userId, Guid id);

    Result<Unit> AddEmail(AddEmailDto addEmailDto);
    Result<Unit> RemoveEmail(Guid userId, Guid contactId, string address);

    Result<Unit> AddPhone(Guid userId, Guid contactId, string number);
    Result<Unit> RemovePhone(Guid userId, Guid contactId, string number);

    Result<Unit> AddImportantDate(Guid userId, Guid contactId, DateOnly date, string description);
    Result<Unit> RemoveImportantDate(Guid userId, Guid contactId, string description);
}
