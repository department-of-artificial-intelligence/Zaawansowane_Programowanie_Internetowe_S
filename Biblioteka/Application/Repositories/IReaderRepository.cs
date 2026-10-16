using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IReaderRepository
    {
        void Add(Reader reader);
        Option<Reader> GetById(Guid id);
        Option<Reader> GetByCardNumber(CardNumber cardNumber);
        IEnumerable<Reader> GetAll();
    }
}
