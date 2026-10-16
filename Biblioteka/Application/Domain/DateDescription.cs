using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public sealed record DateDescription
    {
        public string Value { get; }

        private DateDescription(string value)
        {
            Value = value;
        }

        public static Result<DateDescription> Create(string value) =>
            Validators
                .NotEmpty(value, "Opis ważnej daty nie może być pusty.")
                .Map(_ => new DateDescription(value.Trim()));
    }
}
