using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ContactServices(IContactRepository contactRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork) : IContactUseCases
    {
        public Result<AddContactResult> AddContact(AddContactDto addContactDto) => addContactDto.ToDomain().Tap(contactRepository.Add).Tap(contactRepository.Add).Tap(_ => unitOfWork.Save()).Map(AddContactResult.FromDomain);
        public Result<Unit> AddEmail(AddEmailDto addEmailDto) => contactRepository.GetById(addEmailDto.ContactId).Required($"Brak kontaktu (id: {addEmailDto.CategoryId})").Bind(categoryRepository => param.contact.AddEmail(param.address, category.Id)).Tap(_ => unitOfWork.Save());

        public Result<Unit> AddEmail(AddEmailDto addEmailDto)
    => contactRepository.GetById(addEmailDto.ContactId)
    .Required(
    $"Nie mogę odnaleźć kontaktu o id = {addEmailDto.ContactId}")
    .Bind(contact => EmailAddress.Create(addEmailDto.Address).Map(address => (contact, address)))
    .Bind(param => categoryRepository.GetById(addEmailDto.CategoryId)
    .Required(
    $"Nie mogę odnaleźć kategorii o id = {addEmailDto.CategoryId}")
    .Bind(category => param.contact.AddEmail(
    param.address,
    category.Id)))
    .Tap(_ => unitOfWork.Save());

    }
}