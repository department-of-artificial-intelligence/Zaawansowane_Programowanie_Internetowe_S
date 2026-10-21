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

    public Result<Unit> UpdateContact(UpdateContactDto dto) =>
        contactRepository.GetById(dto.Id)
            .Required($"Brak kontaktu (id: {dto.Id})")
            .Bind(contact => Age.Create(dto.Age)
                .Bind(age => contact.Update(dto.FirstName, dto.LastName, dto.Sex, age)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id) =>
        contactRepository.GetById(id)
            .Required($"Brak kontaktu (id: {id})")
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> RemoveEmail(RemoveEmailDto dto) =>
        contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => EmailAddress.Create(dto.Address)
                .Bind(address => contact.RemoveEmail(address)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhone(AddPhoneDto dto) =>
        contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => PhoneNumber.Create(dto.Number)
                .Bind(number => contact.AddPhone(number, dto.Type)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(RemovePhoneDto dto) =>
        contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => PhoneNumber.Create(dto.Number)
                .Bind(number => contact.RemovePhone(number)))
            .Tap(_ => unitOfWork.Save());


    public Result<Guid> AddImportantDate(AddImportantDateDto dto) =>
        contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => DateLabel.Create(dto.Label)
                .Bind(label => contact.AddImportantDate(label, dto.Date)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> ChangeImportantDate(ChangeImportantDateDto dto) =>
        contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => DateLabel.Create(dto.Label)
                .Bind(label => contact.ChangeImportantDate(dto.DateId, label, dto.Date)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(RemoveImportantDateDto dto) =>
        contactRepository.GetById(dto.ContactId)
            .Required($"Brak kontaktu (id: {dto.ContactId})")
            .Bind(contact => contact.RemoveImportantDate(dto.DateId))
            .Tap(_ => unitOfWork.Save());

}

