using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public class BookCopy : Entity
    {
        public InventoryCode Code { get; private set; }
        public BookCopyStatus Status { get; private set; }

        // Konstruktor dla EF Core
        private BookCopy(Guid id) : base(id)
        {
            Code = null!;
        }

        internal BookCopy(Guid id, InventoryCode code) : base(id)
        {
            Code = code;
            Status = BookCopyStatus.Available;
        }

        public Result<Unit> MarkAsBorrowed()
        {
            if (Status != BookCopyStatus.Available)
            {
                return Result.Failure<Unit>($"Egzemplarz {Code.Value} nie jest dostępny (aktualny status: {Status}).");
            }

            Status = BookCopyStatus.Borrowed;
            return Result.Success(Unit.Instance);
        }

        public Result<Unit> MarkAsReturned()
        {
            Status = BookCopyStatus.Available;
            return Result.Success(Unit.Instance);
        }
    }
}
