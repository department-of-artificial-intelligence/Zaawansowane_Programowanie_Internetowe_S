using System;
using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface ILoanRepository
{
    void Add(Loan loan);
    Option<Loan> GetById(Guid id);
}