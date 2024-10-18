using Contacts.Application;
namespace Contacts.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Save()
        => context.SaveChanges();
}