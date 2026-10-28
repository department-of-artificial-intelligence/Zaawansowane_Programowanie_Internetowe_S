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

    public Result<Unit> UpdateContact(UpdateContactDto dto)
        => FindContact(dto.UserId, dto.ContactId)
            .Bind(contact =>
                Age.Create(dto.Age)
                    .Bind(age => contact.Update(dto.FirstName, dto.LastName, dto.Sex, age)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid userId, Guid id)
        => FindContact(userId, id)
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    // Zadanie 8 - adres email można przypisać tylko do kategorii należącej
    // do tego samego użytkownika, co kontakt.
    public Result<Unit> AddEmail(AddEmailDto addEmailDto)
        => FindContact(addEmailDto.UserId, addEmailDto.ContactId)
            .Bind(contact =>
                EmailAddress
                    .Create(addEmailDto.Address)
                    .Map(address => (contact, address)))
            .Bind(param =>
                categoryRepository
                    .GetById(addEmailDto.CategoryId)
                    .Required($"Brak kategorii (id: {addEmailDto.CategoryId})")
                    .Bind(category => category.UserId == addEmailDto.UserId
                        ? param.contact.AddEmail(param.address, category.Id)
                        : Result.Failure<Unit>($"Brak kategorii (id: {addEmailDto.CategoryId})")))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveEmail(Guid userId, Guid contactId, string address)
        => FindContact(userId, contactId)
            .Bind(contact => EmailAddress.Create(address).Bind(contact.RemoveEmail))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhone(Guid userId, Guid contactId, string number)
        => FindContact(userId, contactId)
            .Bind(contact => PhoneNumber.Create(number).Bind(contact.AddPhone))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(Guid userId, Guid contactId, string number)
        => FindContact(userId, contactId)
            .Bind(contact => PhoneNumber.Create(number).Bind(contact.RemovePhone))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddImportantDate(Guid userId, Guid contactId, DateOnly date, string description)
        => FindContact(userId, contactId)
            .Bind(contact =>
                ImportantDate.Create(date, description).Bind(contact.AddImportantDate))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(Guid userId, Guid contactId, string description)
        => FindContact(userId, contactId)
            .Bind(contact => contact.RemoveImportantDate(description ?? ""))
            .Tap(_ => unitOfWork.Save());

    // Cudzy kontakt jest traktowany tak samo jak nieistniejący,
    // aby nie ujawniać istnienia danych innych użytkowników.
    private Result<Contact> FindContact(Guid userId, Guid id)
        => contactRepository
            .GetByIdAndUser(id, userId)
            .Required($"Brak kontaktu (id: {id})");
}
