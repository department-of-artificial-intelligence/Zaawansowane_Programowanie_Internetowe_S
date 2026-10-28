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

    public Result<Unit> UpdateContact(UpdateContactDto updateContactDto)
        => contactRepository.GetById(updateContactDto.Id)
            .Required($"Brak kontaktu (id: {updateContactDto.Id})")
            .Bind(updateContactDto.ApplyTo)
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
                    .Bind(contact.RemoveEmail))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhoneNumber(AddPhoneNumberDto addPhoneNumberDto)
        => contactRepository.GetById(addPhoneNumberDto.ContactId)
            .Required($"Brak kontaktu (id: {addPhoneNumberDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(addPhoneNumberDto.Number)
                    .Bind(contact.AddPhoneNumber))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhoneNumber(RemovePhoneNumberDto removePhoneNumberDto)
        => contactRepository.GetById(removePhoneNumberDto.ContactId)
            .Required($"Brak kontaktu (id: {removePhoneNumberDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(removePhoneNumberDto.Number)
                    .Bind(contact.RemovePhoneNumber))
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
                ImportantDate
                    .Create(removeImportantDateDto.Description, removeImportantDateDto.Date)
                    .Bind(contact.RemoveImportantDate))
            .Tap(_ => unitOfWork.Save());
}
