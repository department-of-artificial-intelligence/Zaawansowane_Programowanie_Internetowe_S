using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application;

public interface IUnitOfWork
{
    void Save();
}