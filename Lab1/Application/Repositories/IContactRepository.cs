using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IContactRepository{
	IEnumerable<Contact> GetAll();
	Option<Contact> GetById(Guid contactId);
	bool IsCategoryUsed(Guid categoryId);
	void Add(Contact contact);
	void Remove(Contact contact);
}
