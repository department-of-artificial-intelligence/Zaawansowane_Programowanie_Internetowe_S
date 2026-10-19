using Application.OutputPorts;
namespace Data;
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
public void Save()
{
context.SaveChanges();
}
}
