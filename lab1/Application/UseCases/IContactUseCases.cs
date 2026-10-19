
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);
    Result<Unit> AddEmail(AddEmailDto addEmailDto);
    Result<Unit> UpdateContact(UpdateContactDto updateContactDto);
    Result<Unit> RemoveContact(Guid id);
    Result<Unit> RemoveEmail(Guid contactId, string address);

    Result<Unit> AddPhone(Guid contactId, string number);
    Result<Unit> RemovePhone(Guid contactId, string number);

    Result<Unit> AddImportantDate(AddImportantDateDto dto);
    Result<Unit> RemoveImportantDate(Guid contactId, Guid importantDateId);
}
