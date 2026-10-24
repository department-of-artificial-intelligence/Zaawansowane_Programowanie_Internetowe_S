using Application.Domain;
using Application.Repositories;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ContactRepository(AppDbContext context): IContactRepository {
	public void Add(Contact contact){
		context.Contacts.Add(contact);
	}
	
	public IEnumerable<Contact> GetAll() => context.Contacts;

	public bool IsCategoryUsed(Guid categoryId) => context.Contacts.Any(c => c.Emails.Any(e => e.CategoryId == categoryId) || c.Phones.Any(p => p.CategoryId == categoryId));

	public Option<Contact> GetById(Guid contactId){
		var contact = context.Contacts.SingleOrDefault(c => c.Id == contactId);
		return contact is not null ? Option.Some(contact) : Option.None<Contact>();
	}

	public void Remove(Contact contact){
		context.Contacts.Remove(contact);
	}
}
