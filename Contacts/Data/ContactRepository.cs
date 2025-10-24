using Contacts.Application.Domain;
using Contacts.Application.Repositories;
using Microsoft.EntityFrameworkCore; 
using System.Threading.Tasks;

namespace Contacts.Data
{
    public class ContactRepository(AppDbContext context) : IContactRepository
    {
        private readonly AppDbContext _context = context;

        public void Add(Contact contact)
        {
            _context.Contacts.Add(contact);
        }

        public async Task<Contact?> GetByIdAsync(int contactId)
        {
            return await _context.Contacts
                .Include(c => c.Emails)
                .SingleOrDefaultAsync(c => c.Id == contactId);
        }

        public void Remove(Contact contact)
        {
            _context.Contacts.Remove(contact);
        }
    }
}