using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public sealed record Title
    {
        public string Value { get; }

        private Title(string value) => Value = value;

        public static Result<Title> Create(string value) =>
            Validators.NotEmpty(value, "Tytuł książki nie może być pusty.")
                .Map(_ => new Title(value.Trim()));
    }
}
