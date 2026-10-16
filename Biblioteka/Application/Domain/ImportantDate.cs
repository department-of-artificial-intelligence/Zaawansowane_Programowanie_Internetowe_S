using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public sealed record ImportantDate
    {
        public DateOnly Date { get; }
        public DateDescription Description { get; }

        private ImportantDate(DateOnly date, DateDescription description)
        {
            Date = date;
            Description = description;
        }

        internal static Result<ImportantDate> Create(DateOnly date, DateDescription description) =>
            Result.Success(new ImportantDate(date, description));
    }
}
