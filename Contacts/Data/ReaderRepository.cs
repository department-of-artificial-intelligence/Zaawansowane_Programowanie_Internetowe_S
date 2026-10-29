using System;
using System.Linq;
using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class ReaderRepository : IReaderRepository
{
    private readonly AppDbContext _context;

    public ReaderRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Reader reader) => _context.Readers.Add(reader);

    public Option<Reader> GetById(Guid id)
    {
        var reader = _context.Readers.Find(id);
        return reader is not null
            ? Option.Some(reader)
            : Option.None<Reader>();
    }

}