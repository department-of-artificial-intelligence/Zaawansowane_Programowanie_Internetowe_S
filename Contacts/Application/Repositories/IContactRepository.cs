using Contacts.Application.Domain;
using System.Threading.Tasks;

namespace Contacts.Application.Repositories
{
    public interface IContactRepository
    {
        // Pobieramy kontakt razem z jego emailami
        Task<Contact?> GetByIdAsync(int contactId); 
        void Add(Contact contact);
        void Remove(Contact contact);
    }
}