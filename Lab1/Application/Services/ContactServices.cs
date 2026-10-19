using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class ContactServices(IContactRepository contactRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork): IContactUseCases{
	public Result<AddContactResult> AddContact(AddContactDto addContactDto) => addContactDto.ToDomain().Tap(contactRepository.Add).Tap(_ => unitOfWork.Save()).Map(AddContactResult.FromDomain);

	public Result<Unit> AddEmail(AddEmailDto addEmailDto) => 
		contactRepository.GetById(addEmailDto.ContactId).Required($"Brak kontaktu (id: {addEmailDto.ContactId})")
			.Bind(contact => EmailAddress.Create(addEmailDto.Address).Map(address => (contact, address)))
			.Bind(param => categoryRepository.GetById(addEmailDto.CategoryId).Required($"Brak kategorii (id: {addEmailDto.CategoryId})").Bind(category => param.contact.AddEmail(param.address, category.Id)))
			.Tap(_ => unitOfWork.Save());
}
