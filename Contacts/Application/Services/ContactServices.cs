using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using DomainPhoneNumber = Application.Domain.PhoneNumber;

namespace Application.Services;

public class ContactServices(
    IContactRepository contactRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
) : IContactUseCases
{
    public Result<AddContactResult> AddContact(AddContactDto addContactDto)
        => addContactDto
            .ToDomain()
            .Tap(contactRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddContactResult.FromDomain);

    public Result<Unit> UpdateContact(UpdateContactDto updateContactDto)
        => contactRepository.GetById(updateContactDto.Id)
            .Required($"Brak kontaktu (id: {updateContactDto.Id})")
            .Bind(contact => updateContactDto.Validate()
                .Bind(data => contact.Update(data.FirstName, data.LastName, data.Sex, data.Age)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id)
        => contactRepository.GetById(id)
            .Required($"Brak kontaktu (id: {id})")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> AddEmail(AddEmailDto addEmailDto)
        => contactRepository.GetById(addEmailDto.ContactId)
            .Required($"Brak kontaktu (id: {addEmailDto.ContactId})")
            .Bind(contact =>
                EmailAddress
                    .Create(addEmailDto.Address)
                    .Map(address => (Contact: contact, Address: address)))
            .Bind(param =>
                categoryRepository
                    .GetById(addEmailDto.CategoryId)
                    .Required($"Brak kategorii (id: {addEmailDto.CategoryId})")
                    .Bind(category =>
                        param.Contact.AddEmail(param.Address, category.Id)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto)
        => contactRepository.GetById(removeEmailDto.ContactId)
            .Required($"Brak kontaktu (id: {removeEmailDto.ContactId})")
            .Bind(contact =>
                EmailAddress
                    .Create(removeEmailDto.Address)
                    .Bind(contact.RemoveEmail))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhone(AddPhoneDto addPhoneDto)
        => contactRepository.GetById(addPhoneDto.ContactId)
            .Required($"Brak kontaktu (id: {addPhoneDto.ContactId})")
            .Bind(contact =>
                DomainPhoneNumber
                    .Create(addPhoneDto.Number)
                    .Map(number => (Contact: contact, Number: number)))
            .Bind(param =>
                categoryRepository
                    .GetById(addPhoneDto.CategoryId)
                    .Required($"Brak kategorii (id: {addPhoneDto.CategoryId})")
                    .Bind(category =>
                        param.Contact.AddPhone(param.Number, category.Id)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto)
        => contactRepository.GetById(removePhoneDto.ContactId)
            .Required($"Brak kontaktu (id: {removePhoneDto.ContactId})")
            .Bind(contact =>
                DomainPhoneNumber
                    .Create(removePhoneDto.Number)
                    .Bind(contact.RemovePhone))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddImportantDate(AddImportantDateDto addImportantDateDto)
        => contactRepository.GetById(addImportantDateDto.ContactId)
            .Required($"Brak kontaktu (id: {addImportantDateDto.ContactId})")
            .Bind(contact =>
                ImportantDate
                    .Create(addImportantDateDto.Description, addImportantDateDto.Date)
                    .Bind(contact.AddImportantDate))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(RemoveImportantDateDto removeImportantDateDto)
        => contactRepository.GetById(removeImportantDateDto.ContactId)
            .Required($"Brak kontaktu (id: {removeImportantDateDto.ContactId})")
            .Bind(contact =>
                contact.RemoveImportantDate(removeImportantDateDto.Description, removeImportantDateDto.Date))
            .Tap(_ => unitOfWork.Save());
}