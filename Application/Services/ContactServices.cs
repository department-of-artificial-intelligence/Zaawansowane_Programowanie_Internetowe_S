using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public Result<AddContactResult> AddContact(Guid userId, AddContactDto addContactDto)
    => addContactDto
        .ToDomain(userId)
        .Tap(contactRepository.Add)
        .Tap(_ => unitOfWork.Save())
        .Map(AddContactResult.FromDomain);

    public Result<Unit> AddEmail(AddEmailDto addEmailDto)
    => contactRepository.GetById(addEmailDto.ContactId)
        .Required(
            $"Brak kontaktu (id: {addEmailDto.ContactId})")
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

    public Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto)
        => contactRepository.GetById(removeEmailDto.ContactId)
            .Required($"Brak kontaktu (id: {removeEmailDto.ContactId})")
            .Bind(contact =>
                EmailAddress
                    .Create(removeEmailDto.Address)
                    .Map(address => (contact, address)))
            .Bind(param => param.contact.RemoveEmail(param.address))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhone(AddPhoneDto addPhoneDto)
    => contactRepository.GetById(addPhoneDto.ContactId)
        .Required($"Brak kontaktu (id: {addPhoneDto.ContactId})")
        .Bind(contact =>
            PhoneNumber
                .Create(addPhoneDto.PhoneNumber)
                .Map(phone => (contact, phone)))
        .Bind(param => param.contact.AddPhone(param.phone))
        .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto)
        => contactRepository.GetById(removePhoneDto.ContactId)
            .Required($"Brak kontaktu (id: {removePhoneDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(removePhoneDto.PhoneNumber)
                    .Map(phone => (contact, phone)))
            .Bind(param => param.contact.RemovePhone(param.phone))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> UpdateContact(UpdateContactDto updateContactDto)
        => contactRepository.GetById(updateContactDto.Id)
            .Required($"Brak kontaktu o id: {updateContactDto.Id}")
            .Bind(contact =>
                updateContactDto.ToDomainValues()
                    .Map(values => (contact, values)))
            .Bind(param =>
                param.contact.UpdateDetails(param.values.FirstName, param.values.LastName, updateContactDto.Sex, param.values.Age))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id)
        => contactRepository.GetById(id)
            .Required($"Brak kontaktu o id: {id}")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}

