using Contacts.Application;
using Contacts.Data;

namespace Contacts.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Save() => context.SaveChanges();
}
