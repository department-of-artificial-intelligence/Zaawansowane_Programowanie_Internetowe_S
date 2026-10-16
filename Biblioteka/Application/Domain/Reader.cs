using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public class Reader : Entity
    {
        public FirstName FirstName { get; private set; }
        public LastName LastName { get; private set; }
        public EmailAddress Email { get; private set; }
        public CardNumber CardNumber { get; private set; }
        public ReaderStatus Status { get; private set; }
        public int MaxAllowedLoans { get; private set; }

        private Reader(Guid id) : base(id)
        {
            FirstName = null!;
            LastName = null!;
            Email = null!;
            CardNumber = null!;
        }

        private Reader(
            Guid id,
            FirstName firstName,
            LastName lastName,
            EmailAddress email,
            CardNumber cardNumber,
            int maxAllowedLoans = 5) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            CardNumber = cardNumber;
            Status = ReaderStatus.Active;
            MaxAllowedLoans = maxAllowedLoans;
        }

        public static Result<Reader> Create(
            FirstName firstName,
            LastName lastName,
            EmailAddress email,
            CardNumber cardNumber) =>
            Result.Success(new Reader(Guid.NewGuid(), firstName, lastName, email, cardNumber));

        public Result<Unit> EnsureCanBorrow(int currentActiveLoansCount)
        {
            if (Status == ReaderStatus.Blocked)
            {
                return Result.Failure<Unit>("Konto czytelnika jest zablokowane.");
            }

            if (currentActiveLoansCount >= MaxAllowedLoans)
            {
                return Result.Failure<Unit>($"Czytelnik osiągnął maksymalny limit wypożyczeń ({MaxAllowedLoans}).");
            }

            return Result.Success(Unit.Instance);
        }

        public Result<Unit> BlockAccount()
        {
            Status = ReaderStatus.Blocked;
            return Result.Success(Unit.Instance);
        }

        public Result<Unit> UnblockAccount()
        {
            Status = ReaderStatus.Active;
            return Result.Success(Unit.Instance);
        }
    }
}
