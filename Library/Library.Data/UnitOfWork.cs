using Library.Application.OutputPorts;

namespace Library.Data;

public class UnitOfWork(LibraryDbContext context) : IUnitOfWork
{
    public void Save() => context.SaveChanges();
}