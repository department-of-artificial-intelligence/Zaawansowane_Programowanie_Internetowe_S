using System.Linq;
using Contacts.Application;
using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class PhoneServices(IPhoneRepository phoneRepository,
                           IContactRepository contactRepository,
                           ICategoryRepository categoryRepository,
                           IUnitOfWork unitOfWork) : IPhoneUseCases
{
    public AddPhoneResult AddPhone(AddPhoneDTO dto)
    {
        var contact = contactRepository.GetContact(dto.ContactId)
            ?? throw new ServiceException("Kontakt o podanym ID nie istnieje");

        var category = categoryRepository.GetCategory(dto.CategoryId)
            ?? throw new ServiceException("Kategoria o podanym ID nie istnieje");

        var normalized = new PhoneNumber(dto.Number);
        var alreadyExists = phoneRepository.GetPhonesByContact(dto.ContactId)
            .Any(p => ((string)p.Number).Equals((string)normalized, StringComparison.OrdinalIgnoreCase));
        if (alreadyExists)
        {
            throw new ServiceException("Ten numer telefonu jest już przypisany do kontaktu");
        }

        var phone = new Phone(0, contact, category, normalized);
        phoneRepository.Add(phone);
        unitOfWork.Save();
        return (AddPhoneResult)phone;
    }

    public void RemovePhone(int phoneId)
    {
        var phone = phoneRepository.GetPhone(phoneId)
            ?? throw new ServiceException("Numer telefonu o podanym ID nie istnieje");

        phoneRepository.Remove(phone);
        unitOfWork.Save();
    }
}