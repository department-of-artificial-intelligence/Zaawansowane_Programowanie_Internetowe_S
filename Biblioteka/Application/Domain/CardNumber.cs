using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public sealed record CardNumber
    {
        public string Value { get; }

        private CardNumber(string value) => Value = value;

        public static Result<CardNumber> Create(string value) =>
            Validators.NotEmpty(value, "Numer karty czytelnika nie może być pusty.")
                .Map(_ => new CardNumber(value.Trim().ToUpperInvariant()));
    }
}
