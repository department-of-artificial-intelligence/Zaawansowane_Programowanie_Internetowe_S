using System;
using System.Linq;
using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;

    public LoanRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Loan loan) => _context.Loans.Add(loan);

    public Option<Loan> GetById(Guid id)
    {
        var loan = _context.Loans.Find(id);
        return loan is not null
            ? Option.Some(loan)
            : Option.None<Loan>();
    }
}