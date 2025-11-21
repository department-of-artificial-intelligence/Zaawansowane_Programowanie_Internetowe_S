using Contacts.Application.UseCases.DTOs;
using System.Threading.Tasks;

namespace Contacts.Application
{
    public interface IContactUseCases
    {
        Task AddContactAsync(AddContactDTO dto);
        Task UpdateContactAsync(UpdateContactDTO dto);
        Task RemoveContactAsync(int contactId);
        
        Task AddEmailToContactAsync(AddEmailToContactDTO dto);
        Task RemoveEmailFromContactAsync(RemoveEmailFromContactDTO dto);
        
        Task AddPhoneToContactAsync(AddPhoneToContactDTO dto);
        Task RemovePhoneFromContactAsync(RemovePhoneFromContactDTO dto);

        // --- NOWE METODY DLA WAŻNYCH DAT ---
        Task AddImportantDateToContactAsync(AddImportantDateToContactDTO dto);
        Task RemoveImportantDateFromContactAsync(RemoveImportantDateFromContactDTO dto);
    }
}