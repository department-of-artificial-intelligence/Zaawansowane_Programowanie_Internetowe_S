
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);

    Result<Unit> AddEmail(AddEmailDto addEmailDto);

    Result<Unit> RemoveEmail(Guid contactId, string address);

    Result<Unit> AddPhone(AddPhoneDto addPhoneDto);

    Result<Unit> RemovePhone(Guid contactId, string number);

    Result<Unit> AddImportantDate(AddImportantDateDto addImportantDateDto);

    Result<Unit> RemoveImportantDate(
        Guid contactId,
        string name,
        DateOnly date);

    Result<Unit> UpdateContact(UpdateContactDto updateContactDto);

    Result<Unit> RemoveContact(Guid id);
}
