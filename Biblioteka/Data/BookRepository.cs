using Application.Domain;
using Application.Repositories;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class BookRepository(AppDbContext context) : IBookRepository
    {
        public void Add(Book book) => context.Books.Add(book);

        public IEnumerable<Book> GetAll() => context.Books.Include(b => b.Copies).ToList();

        public Option<Book> GetById(Guid id)
        {
            var book = context.Books.Include(b => b.Copies).SingleOrDefault(b => b.Id == id);
            return book is not null ? Option.Some(book) : Option.None<Book>();
        }

        public Option<Book> GetByCopyCode(InventoryCode code)
        {
            var book = context.Books.Include(b => b.Copies).SingleOrDefault(b => b.Copies.Any(c => c.Code == code));
            return book is not null ? Option.Some(book) : Option.None<Book>();
        }
    }
}
