using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IContactUseCases
    {
        Result<AddContactResult> AddContact(AddContactDto addContactDto);
        Result<Unit> AddEmail(AddEmailDto addEmailDto);
    }
}