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
    public class LoanRepository(AppDbContext context) : ILoanRepository
    {
        public void Add(Loan loan) => context.Loans.Add(loan);

        public Option<Loan> GetById(Guid id)
        {
            var loan = context.Loans.SingleOrDefault(l => l.Id == id);
            return loan is not null ? Option.Some(loan) : Option.None<Loan>();
        }

        public Option<Loan> GetActiveLoanByCopyCode(InventoryCode code)
        {
            var loan = context.Loans.FirstOrDefault(l => l.CopyCode == code && l.ReturnDate == null);
            return loan is not null ? Option.Some(loan) : Option.None<Loan>();
        }

        public int CountActiveLoansForReader(Guid readerId) =>
            context.Loans.Count(l => l.ReaderId == readerId && l.ReturnDate == null);
    }
}
