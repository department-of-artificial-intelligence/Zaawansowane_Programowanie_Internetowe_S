using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class ContactServices(IContactRepository ContactRepository, IUnitOfWork unitOfWork)
    : IContactUseCases
{
    public AddContactResult AddContact(AddContactDTO ContactDTO)
    {
        if (ContactRepository.GetContactByName(ContactDTO.FirstName, ContactDTO.LastName) == null)
        {
            ContactRepository.Add(ContactDTO);
            unitOfWork.Save();
            return ContactRepository.GetContactByName(ContactDTO.FirstName, ContactDTO.LastName);
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie ju! istnieje");
        }
    }

    public EditContactResult EditContact(EditContactDTO ContactDTO)
    {
        if (ContactRepository.GetContactByName(ContactDTO.FirstName, ContactDTO.LastName) != null)
        {
            ContactRepository.Edit(ContactDTO);
            unitOfWork.Save();
            return ContactRepository.GetContactByName(ContactDTO.FirstName, ContactDTO.LastName);
        }
        else
        {
            throw new ServiceException("Kategoria o tej nazwie nie istnieje");
        }
    }

    public void RemoveContact(int ContactID)
    {
        var Contact = ContactRepository.GetContact(ContactID);
        if (Contact != null)
        {
            ContactRepository.RemoveContact(Contact);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException($"Kategoria o id: {ContactID} nie istnieje");
        }
    }
}