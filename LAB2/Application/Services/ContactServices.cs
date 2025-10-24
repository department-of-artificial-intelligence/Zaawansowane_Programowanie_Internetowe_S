using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class ContactServices(IContactRepository contactRepository, IUnitOfWork unitOfWork)
    : IContactUseCases
{
    public AddContactResult AddContact(AddContactDTO contactDTO)
    {
        contactRepository.Add(contactDTO);
        unitOfWork.Save();
        return contactRepository.GetContactByName(contactDTO.FirstName, contactDTO.LastName)!;
    }

    public void RemoveContact(int contactID)
    {
        var contact = contactRepository.GetContact(contactID);
        if (contact != null)
        {
            contactRepository.RemoveContact(contact);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException($"Kontakt o id: {contactID} nie istnieje");
        }
    }

    public void UpdateContact(UpdateContactDTO contactDTO)
    {
        var contact = contactRepository.GetContact(contactDTO.Id);
        if (contact != null)
        {
            contactRepository.UpdateContact(contactDTO);
            unitOfWork.Save();
        }
        else
        {
            throw new ServiceException($"Kontakt o id: {contactDTO.Id} nie istnieje");
        }
    }
}
