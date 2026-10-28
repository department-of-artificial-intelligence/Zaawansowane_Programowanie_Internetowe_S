using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);
    Result<Unit> UpdateContact(UpdateContactDto updateContactDto);
    Result<Unit> RemoveContact(Guid id);
    Result<Unit> AddEmail(AddEmailDto addEmailDto);
    Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto);
    Result<Unit> AddPhoneNumber(AddPhoneNumberDto addPhoneNumberDto);
    Result<Unit> RemovePhoneNumber(RemovePhoneNumberDto removePhoneNumberDto);
    Result<Unit> AddImportantDate(AddImportantDateDto addImportantDateDto);
    Result<Unit> RemoveImportantDate(RemoveImportantDateDto removeImportantDateDto);
}
