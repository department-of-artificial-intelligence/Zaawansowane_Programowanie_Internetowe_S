using Contacts.Application;

namespace Contacts.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(AppDbContext context) => this.context = context;
        
        public void Save() => context.SaveChanges();
    }
}
