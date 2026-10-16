using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IContactRepository
    {
        IEnumerable<Contact> GetAll();
        Option<Contact> GetById(Guid contactId);
        bool IsCategoryInUse(Guid categoryId);
        void Add(Contact contact);
        void Remove(Contact contact);
    }

}
