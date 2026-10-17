using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetAll();
    Option<Contact> GetById(Guid contactId);
    void Add(Contact contact);
    void Remove(Contact contact);
}
