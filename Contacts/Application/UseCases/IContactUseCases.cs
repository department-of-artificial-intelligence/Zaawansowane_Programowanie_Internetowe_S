using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.UseCases;

public interface IContactUseCases
{
    AddContactResult AddContact(AddContactDTO contactDTO);
    void UpdateContact(UpdateContactDTO contactDTO);
    void RemoveContact(int contactId);
}