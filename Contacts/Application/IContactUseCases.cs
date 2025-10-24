using Contacts.Application.UseCases.DTOs;
using System.Threading.Tasks;

namespace Contacts.Application
{
    public interface IContactUseCases
    {
        Task AddContactAsync(AddContactDTO dto);
        Task UpdateContactAsync(UpdateContactDTO dto);
        Task RemoveContactAsync(int contactId);
    }
}