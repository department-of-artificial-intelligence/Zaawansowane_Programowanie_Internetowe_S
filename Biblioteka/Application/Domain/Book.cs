using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public class Book : Entity
    {
        public Title Title { get; private set; }
        public Author Author { get; private set; }
        public Isbn Isbn { get; private set; }

        private readonly List<BookCopy> _copies = [];
        public IReadOnlyCollection<BookCopy> Copies => _copies;

        private Book(Guid id) : base(id)
        {
            Title = null!;
            Author = null!;
            Isbn = null!;
        }

        private Book(Guid id, Title title, Author author, Isbn isbn) : base(id)
        {
            Title = title;
            Author = author;
            Isbn = isbn;
        }

        public static Result<Book> Create(Title title, Author author, Isbn isbn) =>
            Result.Success(new Book(Guid.NewGuid(), title, author, isbn));

        public Result<BookCopy> AddCopy(InventoryCode code)
        {
            if (_copies.Any(c => c.Code == code))
            {
                return Result.Failure<BookCopy>($"Egzemplarz o kodzie {code.Value} już istnieje w tej książce.");
            }

            var copy = new BookCopy(Guid.NewGuid(), code);
            _copies.Add(copy);
            return Result.Success(copy);
        }

        public Option<BookCopy> FindCopy(InventoryCode code)
        {
            var copy = _copies.FirstOrDefault(c => c.Code == code);
            return copy is not null ? Option.Some(copy) : Option.None<BookCopy>();
        }
    }
}
