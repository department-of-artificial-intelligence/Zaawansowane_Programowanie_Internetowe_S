using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface IContactUseCases
{
    AddContactResult AddContact(AddContactDTO addContactDTO);
    void RemoveContact(int contactId);
    void UpdateContact(UpdateContactDTO updateContactDTO);
}
