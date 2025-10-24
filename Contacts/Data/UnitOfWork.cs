using Contacts.Application.Repositories;
using System.Threading.Tasks; // Dodaj ten using

namespace Contacts.Data
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}