using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public sealed record Author
    {
        public string Value { get; }

        private Author(string value) => Value = value;

        public static Result<Author> Create(string value) =>
            Validators.NotEmpty(value, "Autor nie może być pusty.")
                .Map(_ => new Author(value.Trim()));
    }
}
