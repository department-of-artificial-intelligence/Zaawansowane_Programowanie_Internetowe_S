using System;
using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IReaderRepository
{
    void Add(Reader reader);
    Option<Reader> GetById(Guid id);
}