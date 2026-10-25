using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);
    Result<Unit> AddEmail(AddEmailDto addEmailDto);
    Result<Unit> UpdateContact(UpdateContactDto dto);
    Result<Unit> RemoveContact(Guid id);
    Result<Unit> RemoveEmail(RemoveEmailDto dto);
    Result<Unit> AddPhone(AddPhoneDto dto);
    Result<Unit> RemovePhone(RemovePhoneDto dto);
    Result<Unit> AddImportantDate(AddImportantDateDto dto);
    Result<Unit> RemoveImportantDate(RemoveImportantDateDto dto);
}