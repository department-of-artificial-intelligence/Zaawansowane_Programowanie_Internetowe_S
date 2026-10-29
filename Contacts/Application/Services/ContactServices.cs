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

    public Result<Unit> UpdateContact(Guid id, UpdateContactDto updateContactDto)
    => contactRepository.GetById(id)
        .Required($"Nie odnaleziono kontaktu o id = {id}")
        .Bind(contact =>
            FirstName.Create(updateContactDto.FirstName)
                .Bind(fn => LastName.Create(updateContactDto.LastName)
                    .Bind(ln => Age.Create(updateContactDto.Age)
                        .Bind(age => contact.UpdateDetails(fn, ln, updateContactDto.Sex, age))
                    )
                )
            )
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> RemoveContact(Guid id)
        => contactRepository.GetById(id)
        .Required($"Nie odnaleziono kontaktu o id = {id}")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto)
    => contactRepository.GetById(removeEmailDto.ContactId)
        .Required($"Nie odnaleziono kontaktu o id = {removeEmailDto.ContactId}")
        .Bind(contact => contact.RemoveEmail(removeEmailDto.EmailAddress))
        .Tap(_ => unitOfWork.Save())
        .Map(_ => Unit.Instance);

    public Result<Unit> AddPhone(AddPhoneDto addPhoneDto)
    => categoryRepository.GetById(addPhoneDto.CategoryId)
        .Required($"Nie odnaleziono kategorii o id = {addPhoneDto.CategoryId}")
        .Bind(_ => contactRepository.GetById(addPhoneDto.ContactId)
            .Required($"Nie odnaleziono kontaktu o id = {addPhoneDto.ContactId}"))
        .Bind(contact => contact.AddPhone(addPhoneDto.Number, addPhoneDto.CategoryId))
        .Tap(_ => unitOfWork.Save())
        .Map(_ => Unit.Instance);

    public Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto)
        => contactRepository.GetById(removePhoneDto.ContactId)
            .Required($"Nie odnaleziono kontaktu o id = {removePhoneDto.ContactId}")
            .Bind(contact => contact.RemovePhone(removePhoneDto.PhoneNumber))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> AddImportantDate(AddImportantDateDto addImportantDateDto)
    {
        return contactRepository.GetById(addImportantDateDto.ContactId)
            .Required($"Nie odnaleziono kontaktu o id = {addImportantDateDto.ContactId}")
            .Bind(contact => contact.AddImportantDate(addImportantDateDto.Name, addImportantDateDto.Date))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
    }

    public Result<Unit> RemoveImportantDate(RemoveImportantDateDto removeImportantDateDto)
    {
        return contactRepository.GetById(removeImportantDateDto.ContactId)
            .Required($"Nie odnaleziono kontaktu o id = {removeImportantDateDto.ContactId}")
            .Bind(contact => contact.RemoveImportantDate(removeImportantDateDto.Name))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
    }
}