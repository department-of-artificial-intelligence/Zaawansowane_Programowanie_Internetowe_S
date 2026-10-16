using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IBookRepository
    {
        void Add(Book book);
        Option<Book> GetById(Guid id);
        Option<Book> GetByCopyCode(InventoryCode code);
        IEnumerable<Book> GetAll();
    }
}
