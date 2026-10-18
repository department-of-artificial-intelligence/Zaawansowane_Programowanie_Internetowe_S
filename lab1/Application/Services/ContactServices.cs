
using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

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

    public Result<Unit> AddEmail(AddEmailDto addEmailDto)
        => contactRepository.GetById(addEmailDto.ContactId)
            .Required($"Brak kontaktu (id: {addEmailDto.ContactId})")
            .Bind(contact =>
                EmailAddress
                    .Create(addEmailDto.Address)
                    .Map(address => (contact, address)))
            .Bind(param =>
                categoryRepository
                    .GetById(addEmailDto.CategoryId)
                    .Required($"Brak kategorii (id: {addEmailDto.CategoryId})")
                    .Bind(category =>
                        param.contact.AddEmail(param.address, category.Id)))
            .Tap(_ => unitOfWork.Save());

            
    public Result<Unit> UpdateContact(UpdateContactDto dto)
    {
        return contactRepository.GetById(dto.Id)
            .Required($"Brak kontaktu (id: {dto.Id})")
            .Bind(contact =>
                Age.Create(dto.Age)
                    .Bind(age => contact.Update(
                        dto.FirstName,
                        dto.LastName,
                        dto.Sex,
                        age)))
            .Tap(_ => unitOfWork.Save());
    }

    public Result<Unit> RemoveContact(Guid id)
    {
        return contactRepository.GetById(id)
            .Required($"Brak kontaktu (id: {id})")
            .Tap(contactRepository.Remove)
            .Map(_ => Unit.Instance)
            .Tap(_ => unitOfWork.Save());
    }


    public Result<Unit> RemoveEmail(Guid contactId, string address)
    {
        return contactRepository.GetById(contactId)
            .Required($"Brak kontaktu (id: {contactId})")
            .Bind(contact =>
                EmailAddress.Create(address)
                    .Bind(emailAddress => contact.RemoveEmail(emailAddress)))
            .Tap(_ => unitOfWork.Save());
    }


    public Result<Unit> AddPhone(Guid contactId, string number)
    {
        return contactRepository.GetById(contactId)
            .Required($"Brak kontaktu (id: {contactId})")
            .Bind(contact =>
                PhoneNumber.Create(number)
                    .Bind(phoneNumber => contact.AddPhone(phoneNumber)))
            .Tap(_ => unitOfWork.Save());
    }

    public Result<Unit> RemovePhone(Guid contactId, string number)
    {
        return contactRepository.GetById(contactId)
            .Required($"Brak kontaktu (id: {contactId})")
            .Bind(contact =>
                PhoneNumber.Create(number)
                    .Bind(phoneNumber => contact.RemovePhone(phoneNumber)))
            .Tap(_ => unitOfWork.Save());
    }


        
    public Result<Unit> AddImportantDate(AddImportantDateDto dto)
    {
        return contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact =>
                EventName.Create(dto.Name)
                    .Bind(name => contact.AddImportantDate(name, dto.Date)))
            .Tap(_ => unitOfWork.Save());
    }

    public Result<Unit> RemoveImportantDate(
        Guid contactId,
        Guid importantDateId)
    {
        return contactRepository.GetById(contactId)
            .Required($"Brak kontaktu (id: {contactId})")
            .Bind(contact => contact.RemoveImportantDate(importantDateId))
            .Tap(_ => unitOfWork.Save());
    }





}
