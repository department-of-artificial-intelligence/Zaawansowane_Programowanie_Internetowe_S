using Application.Domain;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BookServices(IBookRepository bookRepository, IUnitOfWork unitOfWork) : IBookUseCases
    {
        public Result<AddBookResult> AddBook(AddBookDto dto) =>
            Title.Create(dto.Title)
                .Bind(title => Author.Create(dto.Author)
                .Bind(author => Isbn.Create(dto.Isbn)
                .Bind(isbn => Book.Create(title, author, isbn))))
                .Bind(book =>
                {
                    Result<Unit> copiesResult = Result.Success(Unit.Instance);

                    // Funkcyjne dodawanie każdego egzemplarza przez łańcuch Bind
                    foreach (var codeStr in dto.InitialCopies)
                    {
                        copiesResult = copiesResult
                            .Bind(_ => InventoryCode.Create(codeStr))
                            .Bind(code => book.AddCopy(code))
                            .Map(_ => Unit.Instance);
                    }

                    // Jeśli jakikolwiek egzemplarz zwróci błąd, copiesResult przekaże go dalej
                    return copiesResult.Map(_ => book);
                })
                .Tap(bookRepository.Add)
                .Tap(_ => unitOfWork.Save())
                .Map(b => new AddBookResult(b.Id, b.Title.Value, b.Copies.Count));
    }
}
