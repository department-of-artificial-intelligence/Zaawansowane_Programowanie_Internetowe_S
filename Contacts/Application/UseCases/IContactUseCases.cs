using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);
    Result<Unit> AddEmail(AddEmailDto addEmailDto);

    Result<Unit> UpdateContact(Guid id, UpdateContactDto updateContactDto);
    Result<Unit> RemoveContact(Guid id);

    Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto);
}