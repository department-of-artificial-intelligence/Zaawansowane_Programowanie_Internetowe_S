using Contacts.Application;
namespace Contacts.Data;

using System.Threading.Tasks; // Dodaj ten using

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Save()
    => context.SaveChanges();
    // Dodaj tę metodę
    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}