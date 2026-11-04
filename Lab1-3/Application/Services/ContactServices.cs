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
    // ---------- Kontakt ----------

    public Result<AddContactResult> AddContact(AddContactDto addContactDto)
        => addContactDto
            .ToDomain()
            .Tap(contactRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddContactResult.FromDomain);

    public Result<Unit> UpdateContact(UpdateContactDto updateContactDto)
        => contactRepository.GetById(updateContactDto.Id)
            .Required($"Brak kontaktu (id: {updateContactDto.Id})")
            .Bind(contact => updateContactDto
                .ToValueObjects()
                .Tap(values => contact.UpdateDetails(
                    values.firstName,
                    values.lastName,
                    updateContactDto.Sex,
                    values.age)))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> RemoveContact(Guid id)
        => contactRepository.GetById(id)
            .Required($"Brak kontaktu (id: {id})")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // ---------- E-maile ----------

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

    // ---------- Telefony ----------

    public Result<Unit> AddPhone(AddPhoneDto addPhoneDto)
        => contactRepository.GetById(addPhoneDto.ContactId)
            .Required($"Brak kontaktu (id: {addPhoneDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(addPhoneDto.Number)
                    .Bind(number => contact.AddPhone(number, addPhoneDto.Type)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto)
        => contactRepository.GetById(removePhoneDto.ContactId)
            .Required($"Brak kontaktu (id: {removePhoneDto.ContactId})")
            .Bind(contact =>
                PhoneNumber
                    .Create(removePhoneDto.Number)
                    .Bind(contact.RemovePhone))
            .Tap(_ => unitOfWork.Save());

    // ---------- Ważne daty ----------

    public Result<Unit> AddImportantDate(AddImportantDateDto addImportantDateDto)
        => contactRepository.GetById(addImportantDateDto.ContactId)
            .Required($"Brak kontaktu (id: {addImportantDateDto.ContactId})")
            .Bind(contact =>
                DateLabel
                    .Create(addImportantDateDto.Label)
                    .Bind(label => contact.AddImportantDate(
                        addImportantDateDto.Date, label)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(RemoveImportantDateDto removeImportantDateDto)
        => contactRepository.GetById(removeImportantDateDto.ContactId)
            .Required($"Brak kontaktu (id: {removeImportantDateDto.ContactId})")
            .Bind(contact =>
                DateLabel
                    .Create(removeImportantDateDto.Label)
                    .Bind(contact.RemoveImportantDate))
            .Tap(_ => unitOfWork.Save());
}