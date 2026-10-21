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

    // Zadanie 5 - edycja i usuwanie kontaktu.
    public Result<Unit> UpdateContact(UpdateContactDto dto)
        => FindContact(dto.ContactId)
            .Bind(contact =>
                Age.Create(dto.Age)
                    .Bind(age => contact.Update(dto.FirstName, dto.LastName, dto.Sex, age)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id)
        => FindContact(id)
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> AddEmail(AddEmailDto addEmailDto)
        => FindContact(addEmailDto.ContactId)
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

    // Zadanie 6 - usuwanie adresu email.
    public Result<Unit> RemoveEmail(Guid contactId, string address)
        => FindContact(contactId)
            .Bind(contact => EmailAddress.Create(address).Bind(contact.RemoveEmail))
            .Tap(_ => unitOfWork.Save());

    // Zadanie 7 - dołączanie i usuwanie numeru telefonu.
    public Result<Unit> AddPhone(Guid contactId, string number)
        => FindContact(contactId)
            .Bind(contact => PhoneNumber.Create(number).Bind(contact.AddPhone))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(Guid contactId, string number)
        => FindContact(contactId)
            .Bind(contact => PhoneNumber.Create(number).Bind(contact.RemovePhone))
            .Tap(_ => unitOfWork.Save());

    // Zadanie 8 - ważne daty.
    public Result<Unit> AddImportantDate(Guid contactId, DateOnly date, string description)
        => FindContact(contactId)
            .Bind(contact =>
                ImportantDate.Create(date, description).Bind(contact.AddImportantDate))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(Guid contactId, string description)
        => FindContact(contactId)
            .Bind(contact => contact.RemoveImportantDate(description ?? ""))
            .Tap(_ => unitOfWork.Save());

    private Result<Contact> FindContact(Guid id)
        => contactRepository
            .GetById(id)
            .Required($"Brak kontaktu (id: {id})");
}
