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
public Result<Unit> EditContact(EditContactDto dto)
=> contactRepository.GetById(dto.Id)
.Required($"Brak kontaktu (id: {dto.Id})")
.Bind(contact => dto.ToDomain()
.Bind(values => contact.Edit(values.FirstName, values.LastName, dto.Sex, values.Age)))
.Tap(_ => unitOfWork.Save());
public Result<Unit> RemoveContact(Guid id)
=> contactRepository.GetById(id)
.Required($"Brak kontaktu (id: {id})")
.Tap(contactRepository.Remove)
.Tap(_ => unitOfWork.Save())
.Map(_ => Unit.Instance);
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
public Result<Unit> RemoveEmail(RemoveEmailDto dto)
=> contactRepository.GetById(dto.ContactId)
.Required($"Brak kontaktu (id: {dto.ContactId})")
.Bind(contact => EmailAddress.Create(dto.Address)
.Bind(contact.RemoveEmail))
.Tap(_ => unitOfWork.Save());
public Result<Unit> AddPhoneNumber(PhoneNumberDto dto)
=> contactRepository.GetById(dto.ContactId)
.Required($"Brak kontaktu (id: {dto.ContactId})")
.Bind(contact => PhoneNumber.Create(dto.Number)
.Bind(contact.AddPhoneNumber))
.Tap(_ => unitOfWork.Save());
public Result<Unit> RemovePhoneNumber(PhoneNumberDto dto)
=> contactRepository.GetById(dto.ContactId)
.Required($"Brak kontaktu (id: {dto.ContactId})")
.Bind(contact => PhoneNumber.Create(dto.Number)
.Bind(contact.RemovePhoneNumber))
.Tap(_ => unitOfWork.Save());
public Result<Unit> AddImportantDate(ImportantDateDto dto)
=> contactRepository.GetById(dto.ContactId)
.Required($"Brak kontaktu (id: {dto.ContactId})")
.Bind(contact => ImportantDate.Create(dto.Name, dto.Date)
.Bind(contact.AddImportantDate))
.Tap(_ => unitOfWork.Save());
public Result<Unit> RemoveImportantDate(ImportantDateDto dto)
=> contactRepository.GetById(dto.ContactId)
.Required($"Brak kontaktu (id: {dto.ContactId})")
.Bind(contact => ImportantDate.Create(dto.Name, dto.Date)
.Bind(contact.RemoveImportantDate))
.Tap(_ => unitOfWork.Save());
}