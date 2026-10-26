
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
    public Result<AddContactResult> AddContact(
        AddContactDto addContactDto)
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
                    .Map(address => (
                        contact,
                        address,
                        addEmailDto.CategoryId
                    )))
            .Bind(param =>
                categoryRepository
                    .GetById(param.CategoryId)
                    .Required(
                        $"Brak kategorii (id: {param.CategoryId})")
                    .Bind(category =>
                        param.contact.AddEmail(
                            param.address,
                            category.Id)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveEmail(Guid contactId, string address)
        => contactRepository.GetById(contactId)
            .Required($"Brak kontaktu (id: {contactId})")
            .Bind(contact =>
                EmailAddress.Create(address)
                    .Bind(emailAddress =>
                        contact.RemoveEmail(emailAddress)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhone(AddPhoneDto addPhoneDto)
        => contactRepository.GetById(addPhoneDto.ContactId)
            .Required(
                $"Brak kontaktu (id: {addPhoneDto.ContactId})")
            .Bind(contact =>
                PhoneNumber.Create(addPhoneDto.Number)
                    .Bind(number =>
                        contact.AddPhone(number)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(Guid contactId, string number)
        => contactRepository.GetById(contactId)
            .Required($"Brak kontaktu (id: {contactId})")
            .Bind(contact =>
                PhoneNumber.Create(number)
                    .Bind(phoneNumber =>
                        contact.RemovePhone(phoneNumber)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddImportantDate(
        AddImportantDateDto addImportantDateDto)
        => contactRepository.GetById(addImportantDateDto.ContactId)
            .Required(
                $"Brak kontaktu (id: {addImportantDateDto.ContactId})")
            .Bind(contact =>
                contact.AddImportantDate(
                    addImportantDateDto.Name,
                    addImportantDateDto.Date))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(
        Guid contactId,
        string name,
        DateOnly date)
        => contactRepository.GetById(contactId)
            .Required($"Brak kontaktu (id: {contactId})")
            .Bind(contact =>
                contact.RemoveImportantDate(name, date))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> UpdateContact(
        UpdateContactDto updateContactDto)
        => contactRepository.GetById(updateContactDto.Id)
            .Required(
                $"Brak kontaktu (id: {updateContactDto.Id})")
            .Bind(contact =>
                Age.Create(updateContactDto.Age)
                    .Bind(age =>
                        contact.Update(
                            updateContactDto.FirstName,
                            updateContactDto.LastName,
                            updateContactDto.Sex,
                            age)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id)
        => contactRepository.GetById(id)
            .Required($"Brak kontaktu (id: {id})")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
}
