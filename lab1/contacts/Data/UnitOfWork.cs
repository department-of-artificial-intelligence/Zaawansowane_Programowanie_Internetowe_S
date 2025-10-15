using Application;
using Data;

namespace Contacts.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Save()
        => context.SaveChanges();
}
