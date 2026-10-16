using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Utilities;

namespace Application.Domain
{
    public sealed record FirstName
    {
        public string Value { get; }

        private FirstName(string value)
        {
            Value = value;
        }

        public static Result<FirstName> Create(string value) =>
            Validators
                .NotEmpty(value, "Imię jest wymagane.")
                .Map(_ => new FirstName(value));
    }
}
