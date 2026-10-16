using Application.Domain;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ILoanRepository
    {
        void Add(Loan loan);
        Option<Loan> GetById(Guid id);
        Option<Loan> GetActiveLoanByCopyCode(InventoryCode code);
        int CountActiveLoansForReader(Guid readerId);
    }
}
