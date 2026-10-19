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
                    .Map(address => (contact, address))
            )
            .Bind(param =>
                categoryRepository
                    .GetById(addEmailDto.CategoryId)
                    .Required($"Brak kategorii (id: {addEmailDto.CategoryId})")
                    .Bind(category =>
                        param.contact.AddEmail(param.address, category.Id))
            )
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto)
        => contactRepository.GetById(removeEmailDto.ContactId)
            .Required(
                $"Brak kontaktu (id: {removeEmailDto.ContactId})")
            .Bind(contact =>
                EmailAddress
                    .Create(removeEmailDto.EmailAddress)
                    .Bind(address => contact.RemoveEmail(address))
            )
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id)
        => contactRepository.GetById(id)
            .Required($"Nie mogę odnaleźć kontaktu o id = {id}")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> UpdateContact(UpdateContactDto updateContactDto)
        => contactRepository.GetById(updateContactDto.Id)
            .Required($"Nie mogę odnaleźć kontakt o id = {updateContactDto.Id}")
            .Bind(contact =>
                FirstName.Create(updateContactDto.FirstName)
                    .Bind(firstName => LastName.Create(updateContactDto.LastName)
                        .Bind(lastName => Age.Create(updateContactDto.Age)
                            .Bind(age => contact.UpdateDetails(firstName, lastName, updateContactDto.Sex, age))
                        )
                    )
            )
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> AddPhone(AddPhoneDto addPhoneDto)
        => contactRepository.GetById(addPhoneDto.ContactId)
            .Required(
                $"Brak kontaktu (id: {addPhoneDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(addPhoneDto.PhoneNumber)
                    .Bind(phone => contact.AddPhone(phone))
            )
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto)
    => contactRepository.GetById(removePhoneDto.ContactId)
        .Required($"Brak kontaktu (id: {removePhoneDto.ContactId})")
        .Bind(contact =>
            PhoneNumber
                .Create(removePhoneDto.PhoneNumber)
                .Bind(phone => contact.RemovePhone(phone))
        )
        .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddImportantDate(AddImportantDateDto addImportantDateDto)
        => contactRepository.GetById(addImportantDateDto.ContactId)
            .Required(
                $"Brak kontaktu (id: {addImportantDateDto.ContactId})")
            .Bind(contact =>
                ImportantDate
                    .Create(addImportantDateDto.Date, addImportantDateDto.Description)
                    .Bind(date => contact.AddImportantDate(date))
            )
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(RemoveImportantDateDto removeImportantDateDto)
        => contactRepository.GetById(removeImportantDateDto.ContactId)
            .Required($"Brak kontaktu (id: {removeImportantDateDto.ContactId})")
            .Bind(contact =>
                ImportantDate
                    .Create(removeImportantDateDto.Date, removeImportantDateDto.Description)
                    .Bind(date => contact.RemoveImportantDate(date))
            )
            .Tap(_ => unitOfWork.Save());
}