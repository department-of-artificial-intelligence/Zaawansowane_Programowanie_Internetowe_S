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
            .Required(
                $"Brak kontaktu (id: {removeEmailDto.ContactId})")
            .Bind(contact =>
                EmailAddress
                    .Create(removeEmailDto.Address)
                    .Bind(address => contact.RemoveEmail(address)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> EditContact(EditContactDto editContactDto)
        => contactRepository.GetById(editContactDto.contactId)
            .Required($"Nie mogę odnaleźć kontaktu o id = {editContactDto.contactId}")
            .Edit(
                editContactDto.firstName,
                editContactDto.lastName,
                editContactDto.sex,
                editContactDto.age
            )
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid contactId)
        => contactRepository.GetById(contactId)
            .Required($"Nie mogę odnaleźć kontaktu o id = {contactId}")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> AddEmail(AddPhoneNumberDto addPhoneNumberDto)
        => contactRepository.GetById(addPhoneNumberDto.ContactId)
            .Required(
                $"Brak kontaktu (id: {addPhoneNumberDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(addPhoneNumberDto.Number)
                    .Map(number => (contact, number)))
            .Tap(_ => unitOfWork.Save());

    Result<Unit> RemovePhoneNumber(RemovePhoneNumberDto removePhoneNumberDto)
        => contactRepository.GetById(removePhoneNumberDto.ContactId)
            .Required(
                $"Brak kontaktu (id: {removePhoneNumberDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(removePhoneNumberDto.Number)
                    .Bind(number => contact.RemovePhoneNumber(number)))
            .Tap(_ => unitOfWork.Save());
}
