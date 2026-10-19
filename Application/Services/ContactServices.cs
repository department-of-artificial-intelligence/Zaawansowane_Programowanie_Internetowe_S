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

    public Result<Unit> UpdateContact(UpdateContactDto dto)
    => FirstName.Create(dto.FirstName)
        .Bind(firstName => LastName.Create(dto.LastName)
            .Bind(lastName => Age.Create(dto.Age)
                .Bind(age => contactRepository.GetById(dto.Id)
                    .Required($"Nie odnaleziono kontaktu o id = {dto.Id}")
                    .Bind(contact => contact.Update(firstName, lastName, dto.Sex, age))
                )
            )
        )
        .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id)
        => contactRepository.GetById(id)
            .Required($"Nie odnaleziono kontaktu o id = {id}")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);
            
    public Result<Unit> RemoveEmail(RemoveEmailDto dto)
    => EmailAddress.Create(dto.EmailAddress)
        .Bind(emailAddress => contactRepository.GetById(dto.ContactId)
            .Required($"Nie odnaleziono kontaktu o id = {dto.ContactId}")
            .Bind(contact => contact.RemoveEmail(emailAddress))
        )
        .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhone(AddPhoneDto dto)
    => PhoneNumber.Create(dto.PhoneNumber)
        .Bind(phoneNum => contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => categoryRepository.GetById(dto.CategoryId)
                .Required($"Brak kategorii (id: {dto.CategoryId})")
                .Bind(category => contact.AddPhone(phoneNum, category.Id))
            )
        )
        .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(RemovePhoneDto dto)
        => PhoneNumber.Create(dto.PhoneNumber)
            .Bind(phoneNum => contactRepository.GetById(dto.ContactId)
                .Required($"Brak kontaktu (id: {dto.ContactId})")
                .Bind(contact => contact.RemovePhone(phoneNum))
            )
            .Tap(_ => unitOfWork.Save());
    
    public Result<Unit> AddImportantDate(AddImportantDateDto dto)
    => EventName.Create(dto.Name)
        .Bind(eventName => contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => contact.AddImportantDate(eventName, dto.Date))
        )
        .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(RemoveImportantDateDto dto)
        => EventName.Create(dto.Name)
            .Bind(eventName => contactRepository.GetById(dto.ContactId)
                .Required($"Brak kontaktu (id: {dto.ContactId})")
                .Bind(contact => contact.RemoveImportantDate(eventName))
            )
            .Tap(_ => unitOfWork.Save());
}