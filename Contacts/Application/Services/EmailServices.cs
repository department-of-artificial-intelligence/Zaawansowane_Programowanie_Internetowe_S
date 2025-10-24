using System.Linq;
using Contacts.Application;
using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;

namespace Contacts.Application.Services;

public class EmailServices(IEmailRepository emailRepository,
                           IContactRepository contactRepository,
                           ICategoryRepository categoryRepository,
                           IUnitOfWork unitOfWork) : IEmailUseCases
{
    public AddEmailResult AddEmail(AddEmailDTO dto)
    {
        var contact = contactRepository.GetContact(dto.ContactId)
            ?? throw new ServiceException("Kontakt o podanym ID nie istnieje");

        var category = categoryRepository.GetCategory(dto.CategoryId)
            ?? throw new ServiceException("Kategoria o podanym ID nie istnieje");

        // Prosta kontrola duplikatów: ten sam adres w ramach tego kontaktu
        var alreadyExists = emailRepository.GetEmailsByContact(dto.ContactId)
            .Any(e => ((string)e.Address).Equals(dto.Address, StringComparison.OrdinalIgnoreCase));
        if (alreadyExists)
        {
            throw new ServiceException("Ten adres email jest już przypisany do kontaktu");
        }

        var email = new Email(0, contact, category, new EmailAddress(dto.Address));
        emailRepository.Add(email);
        unitOfWork.Save();
        return (AddEmailResult)email;
    }

    public void RemoveEmail(int emailId)
    {
        var email = emailRepository.GetEmail(emailId)
            ?? throw new ServiceException("Adres email o podanym ID nie istnieje");

        emailRepository.Remove(email);
        unitOfWork.Save();
    }
}