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
    public class ReaderServices(IReaderRepository readerRepository, IUnitOfWork unitOfWork) : IReaderUseCases
    {
        public Result<RegisterReaderResult> RegisterReader(RegisterReaderDto dto) =>
            FirstName.Create(dto.FirstName)
                .Bind(fn => LastName.Create(dto.LastName)
                .Bind(ln => EmailAddress.Create(dto.Email)
                .Bind(email => CardNumber.Create(dto.CardNumber)
                .Bind(card =>
                    readerRepository.GetByCardNumber(card).Match(
                        _ => Result.Failure<Reader>($"Czytelnik z kartą '{card.Value}' już istnieje."),
                        () => Reader.Create(fn, ln, email, card))))))
                .Tap(readerRepository.Add)
                .Tap(_ => unitOfWork.Save())
                .Map(r => new RegisterReaderResult(r.Id, $"{r.FirstName.Value} {r.LastName.Value}", r.CardNumber.Value));
    }
}
