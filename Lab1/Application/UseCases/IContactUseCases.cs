using Application.UseCases.Dtos;
using Application.Utilities;
namespace Application.UseCases;
public interface IContactUseCases
{
Result<AddContactResult> AddContact(AddContactDto addContactDto);
Result<Unit> EditContact(EditContactDto editContactDto);
Result<Unit> RemoveContact(Guid id);
Result<Unit> AddEmail(AddEmailDto addEmailDto);
Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto);
Result<Unit> AddPhoneNumber(PhoneNumberDto phoneNumberDto);
Result<Unit> RemovePhoneNumber(PhoneNumberDto phoneNumberDto);
Result<Unit> AddImportantDate(ImportantDateDto importantDateDto);
Result<Unit> RemoveImportantDate(ImportantDateDto importantDateDto);
}