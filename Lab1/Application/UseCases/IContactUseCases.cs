using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.UseCases.Dtos;


namespace Application.UseCases
{
    public interface IContactUseCases
    {
        Result<AddContactResult> AddContact(AddContactDto addContactDto);
        Result<Unit> AddEmail(AddEmailDto addEmailDto);
        Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto); 
        Result<Unit> UpdateContact(UpdateContactDto updateContactDto);
        Result<Unit> RemoveContact(Guid id);
        Result<Unit> AddPhone(AddPhoneDto addPhoneDto);
        Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto);
        Result<Unit> AddImportantDate(AddImportantDateDto addImportantDateDto);
        Result<Unit> RemoveImportantDate(RemoveImportantDateDto removeImportantDateDto);
    }

}
