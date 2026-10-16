using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public class Loan : Entity
    {
        public Guid ReaderId { get; private set; }
        public Guid BookId { get; private set; }
        public InventoryCode CopyCode { get; private set; }
        public DateOnly BorrowDate { get; private set; }
        public DateOnly DueDate { get; private set; }
        public DateOnly? ReturnDate { get; private set; }
        public int ExtensionCount { get; private set; }

        private Loan(Guid id) : base(id)
        {
            CopyCode = null!;
        }

        private Loan(
            Guid id,
            Guid readerId,
            Guid bookId,
            InventoryCode copyCode,
            DateOnly borrowDate,
            int durationDays) : base(id)
        {
            ReaderId = readerId;
            BookId = bookId;
            CopyCode = copyCode;
            BorrowDate = borrowDate;
            DueDate = borrowDate.AddDays(durationDays);
            ExtensionCount = 0;
        }

        public static Result<Loan> Create(
            Guid readerId,
            Guid bookId,
            InventoryCode copyCode,
            DateOnly borrowDate,
            int durationDays = 30) =>
            Result.Success(new Loan(Guid.NewGuid(), readerId, bookId, copyCode, borrowDate, durationDays));

        public Result<Unit> Extend(int additionalDays)
        {
            if (ReturnDate.HasValue)
            {
                return Result.Failure<Unit>("Nie można przedłużyć zakończonego wypożyczenia.");
            }

            if (ExtensionCount >= 1)
            {
                return Result.Failure<Unit>("Limit przedłużeń dla tego wypożyczenia został wyczerpany (maksymalnie 1).");
            }

            DueDate = DueDate.AddDays(additionalDays);
            ExtensionCount++;
            return Result.Success(Unit.Instance);
        }

        public Result<Unit> CompleteReturn(DateOnly returnDate)
        {
            if (ReturnDate.HasValue)
            {
                return Result.Failure<Unit>("To wypożyczenie zostało już wcześniej rozliczone.");
            }

            ReturnDate = returnDate;
            return Result.Success(Unit.Instance);
        }

        public bool IsActive => ReturnDate == null;
    }
}
