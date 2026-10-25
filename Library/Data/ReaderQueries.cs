using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
using Application.Domain;
using Application.UseCases;

namespace Data;

public class ReaderQueries(AppDbContext context) : IReaderQueries
{
    public IEnumerable<ReaderDto> GetAll()
        => context.Readers.ToList().Select(ReaderDto.FromDomain);

    public Option<ReaderDto> GetById(Guid id)
    {
        var reader = context.Readers.Find(id);
        return reader is not null
            ? Option.Some(ReaderDto.FromDomain(reader))
            : Option.None<ReaderDto>();
    }
}