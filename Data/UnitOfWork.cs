using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Save() => context.SaveChanges();
}
