using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application;

namespace Contacts.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public void Save()
        => context.SaveChanges();
}