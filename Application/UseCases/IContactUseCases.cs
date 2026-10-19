using Application.UseCases.Dtos;
using Application.Utilities;
namespace Application.UseCases;

public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);
    Result<Unit> AddEmail(AddEmailDto addEmailDto);
}

