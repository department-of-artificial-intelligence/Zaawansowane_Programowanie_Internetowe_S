using Application.Domain;
using Application.Repositories;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ReaderRepository(AppDbContext context) : IReaderRepository
    {
        public void Add(Reader reader) => context.Readers.Add(reader);

        public IEnumerable<Reader> GetAll() => context.Readers.ToList();

        public Option<Reader> GetById(Guid id)
        {
            var reader = context.Readers.SingleOrDefault(r => r.Id == id);
            return reader is not null ? Option.Some(reader) : Option.None<Reader>();
        }

        public Option<Reader> GetByCardNumber(CardNumber cardNumber)
        {
            var reader = context.Readers.SingleOrDefault(r => r.CardNumber == cardNumber);
            return reader is not null ? Option.Some(reader) : Option.None<Reader>();
        }
    }
}
