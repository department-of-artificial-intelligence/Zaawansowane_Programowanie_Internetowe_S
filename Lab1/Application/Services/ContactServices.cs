using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class ContactServices(IContactRepository contactRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork): IContactUseCases{
	public Result<AddContactResult> AddContact(AddContactDto addContactDto) => addContactDto.ToDomain().Tap(contactRepository.Add).Tap(_ => unitOfWork.Save()).Map(AddContactResult.FromDomain);

	public Result<Unit> UpdateContact(UpdateContactDto dto) => 
		contactRepository.GetById(dto.Id)
        	.Required($"Brak kontaktu (id: {dto.Id})")
        	.Bind(contact => FirstName.Create(dto.FirstName)
        	    .Bind(firstName => LastName.Create(dto.LastName)
                    .Map(lastName => (firstName, lastName)))
    	        .Bind(names => Age.Create(dto.Age)
                    .Bind(age => contact.Update(names.firstName, names.lastName, dto.Sex, age)))
        )
        .Tap(_ => unitOfWork.Save());

	public Result<Unit> RemoveContact(Guid id) => 
		contactRepository.GetById(id)
        	.Required($"Brak kontaktu (id: {id})")
        	.Tap(contactRepository.Remove)
        	.Tap(_ => unitOfWork.Save())
        	.Map(_ => Unit.Instance);

	public Result<Unit> RemoveEmail(RemoveEmailDto removeEmailDto) => 
		contactRepository.GetById(removeEmailDto.ContactId)
        	.Required($"Brak kontaktu (id: {removeEmailDto.ContactId})")
        	.Bind(contact => EmailAddress
                .Create(removeEmailDto.Address)
                .Bind(contact.RemoveEmail))
        	.Tap(_ => unitOfWork.Save());

	public Result<Unit> AddEmail(AddEmailDto addEmailDto) => 
		contactRepository.GetById(addEmailDto.ContactId).Required($"Brak kontaktu (id: {addEmailDto.ContactId})")
			.Bind(contact => EmailAddress.Create(addEmailDto.Address).Map(address => (contact, address)))
			.Bind(param => categoryRepository.GetById(addEmailDto.CategoryId).Required($"Brak kategorii (id: {addEmailDto.CategoryId})").Bind(category => param.contact.AddEmail(param.address, category.Id)))
			.Tap(_ => unitOfWork.Save());

	public Result<Unit> AddPhone(AddPhoneDto addPhoneDto) => 
		contactRepository.GetById(addPhoneDto.ContactId)
        	.Required($"Brak kontaktu (id: {addPhoneDto.ContactId})")
        	.Bind(contact => PhoneNumber
                .Create(addPhoneDto.Number)
                .Map(number => (contact, number)))
        	.Bind(param => categoryRepository
                .GetById(addPhoneDto.CategoryId)
                .Required($"Brak kategorii (id: {addPhoneDto.CategoryId})")
                .Bind(category => param.contact.AddPhone(param.number, category.Id)))
        	.Tap(_ => unitOfWork.Save());

	public Result<Unit> RemovePhone(RemovePhoneDto removePhoneDto) => 
		contactRepository.GetById(removePhoneDto.ContactId)
        	.Required($"Brak kontaktu (id: {removePhoneDto.ContactId})")
        	.Bind(contact => PhoneNumber
                .Create(removePhoneDto.Number)
                .Bind(contact.RemovePhone))
        	.Tap(_ => unitOfWork.Save());

	public Result<Unit> AddImportantDate(AddImportantDateDto dto) => 
		contactRepository.GetById(dto.ContactId)
        	.Required($"Brak kontaktu (id: {dto.ContactId})")
        	.Bind(contact => contact.AddImportantDate(dto.Description, dto.Date))
        	.Tap(_ => unitOfWork.Save());

	public Result<Unit> RemoveImportantDate(RemoveImportantDateDto dto) => 
		contactRepository.GetById(dto.ContactId)
    		.Required($"Brak kontaktu (id: {dto.ContactId})")
        	.Bind(contact => contact.RemoveImportantDate(dto.Description, dto.Date))
        	.Tap(_ => unitOfWork.Save());
}
