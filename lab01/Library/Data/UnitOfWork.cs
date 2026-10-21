using Application.OutputPorts;

namespace Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Save()
    {
        context.SaveChanges();
    }
}

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}
