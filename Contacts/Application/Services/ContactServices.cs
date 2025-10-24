using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;
using Contacts.Application; // for IUnitOfWork
using Contacts.Application.Domain; // for Contact

namespace Contacts.Application.Services;

public class ContactServices(IContactRepository contactRepository, IUnitOfWork unitOfWork) : IContactUseCases
{
    public AddContactResult AddContact(AddContactDTO contactDTO)
    {
        var contact = (Contact)contactDTO;
        contactRepository.Add(contact);
        unitOfWork.Save();
        return (AddContactResult)contact;
    }

    public void UpdateContact(UpdateContactDTO contactDTO)
    {
        var contact = contactRepository.GetContact(contactDTO.Id);
        if (contact is null)
            throw new ServiceException("Kontakt o podanym ID nie istnieje");

        contact.UpdateFirstname(contactDTO.Firstname);
        contact.UpdateLastName(contactDTO.LastName);
        contact.UpdateSex(contactDTO.Sex);
        contact.UpdateAge(new Age(contactDTO.Age));

        contactRepository.Update(contact);
        unitOfWork.Save();
    }

    public void RemoveContact(int contactId)
    {
        var contact = contactRepository.GetContact(contactId);
        if (contact is null)
            throw new ServiceException("Kontakt o podanym ID nie istnieje");

        contactRepository.Remove(contact);
        unitOfWork.Save();
    }
}