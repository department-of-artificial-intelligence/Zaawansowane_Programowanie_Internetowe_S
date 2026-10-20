using Application.UseCases.Dtos;
using Application.Utilities;
namespace Application.UseCases;
public interface IContactUseCases
{
Result<AddContactResult> AddContact(AddContactDto addContactDto);
Result<Unit> AddEmail(AddEmailDto addEmailDto);

Result<Unit> UpdateContact(UpdateContactDto updateContactDto);
Result<Unit> RemoveContact(Guid id);

Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto);

Result<Unit> AddPhone(AddPhoneDto addPhoneDto);
Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto);

Result<Guid> AddImportantDate(AddImportantDateDto dto);
Result<Unit> ChangeImportantDate(ChangeImportantDateDto dto);
Result<Unit> RemoveImportantDate(RemoveImportantDateDto dto);
}
