using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Domain
{
    public record FirstName
    {
        public string Value { get; }

        public FirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("First name cannot be empty.");
            }

            if (value.Length > 50)
            {
                throw new ArgumentException("First name cannot exceed 50 characters.");
            }

            Value = value;
        }

        public static implicit operator string(FirstName firstName) => firstName.Value;
        public static implicit operator FirstName(string value) => new(value);
    }
}