using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Domain
{
    public record LastName
    {
        public string Value { get; }

        public LastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Last name cannot be empty.");
            }

            if (value.Length > 50)
            {
                throw new ArgumentException("Last name cannot exceed 50 characters.");
            }

            Value = value;
        }

        public static implicit operator string(LastName lastName) => lastName.Value;
        public static implicit operator LastName(string value) => new(value);
    }
}