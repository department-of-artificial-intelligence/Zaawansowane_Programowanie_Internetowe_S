using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Domain
{
    public sealed record InventoryCode
    {
        private static readonly Regex CodeRegex = new(@"^[A-Z0-9-]{4,20}$", RegexOptions.Compiled);

        public string Value { get; }

        private InventoryCode(string value) => Value = value;

        public static Result<InventoryCode> Create(string value) =>
            Validators.NotEmpty(value, "Kod egzemplarza nie może być pusty.")
                .Bind(_ => CodeRegex.IsMatch(value.Trim().ToUpperInvariant())
                    ? Result.Success(new InventoryCode(value.Trim().ToUpperInvariant()))
                    : Result.Failure<InventoryCode>($"Niepoprawny format kodu egzemplarza: '{value}'."));
    }
}
