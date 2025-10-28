using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface IContactUseCases
{
    AddContactResult AddContact(AddContactDTO addContactDTO);
    EditContactResult EditContact(EditContactDTO editContactDTO);
    void RemoveContact(int contactId);
}