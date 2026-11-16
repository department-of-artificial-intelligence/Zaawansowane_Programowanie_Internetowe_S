using System;

namespace Application.Domain
{
    public record FirstName
    {
        public string Value { get; }

        public FirstName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Imię jest wymagane.", nameof(value));
            }

            Value = value.Trim();
        }

        public static Result<FirstName> Create(string value) =>
            Validators.NotEmpty(value, "Imię jest wymagane.")
                .Map(v => new FirstName(v));

        public static implicit operator string(FirstName firstName) => firstName.Value;
        public static explicit operator FirstName(string value) => new(value);

        public override string ToString() => Value;
    }

    public record LastName
    {
        public string Value { get; }

        public LastName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Nazwisko jest wymagane.", nameof(value));
            }

            Value = value.Trim();
        }

        public static Result<LastName> Create(string value) =>
            Validators.NotEmpty(value, "Nazwisko jest wymagane.")
                .Map(v => new LastName(v));

        public static implicit operator string(LastName lastName) => lastName.Value;
        public static explicit operator LastName(string value) => new(value);

        public override string ToString() => Value;
    }
}