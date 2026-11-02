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
    public Result<AddContactResult> AddContact(AddContactDto addContactDto) =>
        addContactDto
            .ToDomain()
            .Tap(contactRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddContactResult.FromDomain);

    public Result<Unit> UpdateContact(UpdateContactDto dto) =>
        GetOwnedContact(dto.Id, dto.UserId)
            .Bind(contact => Age.Create(dto.Age)
                .Bind(age => contact.Update(dto.FirstName, dto.LastName, dto.Sex, age)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveContact(Guid id, Guid userId) =>
        GetOwnedContact(id, userId)
            .Tap(contactRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => Unit.Instance);

    public Result<Unit> AddEmail(AddEmailDto dto) =>
        GetOwnedContact(dto.ContactId, dto.UserId)
            .Bind(contact => EmailAddress
                .Create(dto.Address)
                .Map(address => (contact, address)))
            .Bind(param => GetOwnedCategory(dto.CategoryId, dto.UserId)
                .Bind(category => param.contact.AddEmail(param.address, category.Id)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveEmail(RemoveEmailDto dto) =>
        GetOwnedContact(dto.ContactId, dto.UserId)
            .Bind(contact => EmailAddress.Create(dto.Address)
                .Bind(address => contact.RemoveEmail(address)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> AddPhone(AddPhoneDto dto) =>
        GetOwnedContact(dto.ContactId, dto.UserId)
            .Bind(contact => PhoneNumber.Create(dto.Number)
                .Bind(number => contact.AddPhone(number, dto.Type)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemovePhone(RemovePhoneDto dto) =>
        GetOwnedContact(dto.ContactId, dto.UserId)
            .Bind(contact => PhoneNumber.Create(dto.Number)
                .Bind(number => contact.RemovePhone(number)))
            .Tap(_ => unitOfWork.Save());

    public Result<Guid> AddImportantDate(AddImportantDateDto dto) =>
        GetOwnedContact(dto.ContactId, dto.UserId)
            .Bind(contact => DateLabel.Create(dto.Label)
                .Bind(label => contact.AddImportantDate(label, dto.Date)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> ChangeImportantDate(ChangeImportantDateDto dto) =>
        GetOwnedContact(dto.ContactId, dto.UserId)
            .Bind(contact => DateLabel.Create(dto.Label)
                .Bind(label => contact.ChangeImportantDate(dto.DateId, label, dto.Date)))
            .Tap(_ => unitOfWork.Save());

    public Result<Unit> RemoveImportantDate(RemoveImportantDateDto dto) =>
        GetOwnedContact(dto.ContactId, dto.UserId)
            .Bind(contact => contact.RemoveImportantDate(dto.DateId))
            .Tap(_ => unitOfWork.Save());

    // Zadanie 8: cudzy kontakt wygląda tak, jakby nie istniał (404, a nie 403),
    // żeby nie zdradzać, że kontakt o takim id w ogóle jest w bazie
    private Result<Contact> GetOwnedContact(Guid contactId, Guid userId) =>
        contactRepository.GetById(contactId, userId)
            .Required($"Brak kontaktu (id: {contactId})");

    // Adres e-mail można przypisać tylko do własnej kategorii
    private Result<Category> GetOwnedCategory(Guid categoryId, Guid userId) =>
        categoryRepository.GetById(categoryId)
            .Required($"Brak kategorii (id: {categoryId})")
            .Bind(category => category.UserId == userId
                ? Result.Success(category)
                : Result.NotFound<Category>($"Brak kategorii (id: {categoryId})"));
}
