using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IContactUseCases
{
    Result<AddContactResult> AddContact(AddContactDto addContactDto);
    Result<Unit> AddEmail(AddEmailDto addEmailDto);
    Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto);
    Result<Unit> AddPhone(AddPhoneDto addPhoneDto);    
    Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto);
    Result<Unit> UpdateContact(UpdateContactDto updateContactDto);
    Result<Unit> RemoveContact(Guid id);
}
