using System;

namespace Contacts.Application.Domain
{
    // Obiekt wartości dla opisu daty
    public record DateDescriptionVO
    {
        public string Value { get; private set; } = null!; // null! dla EF Core

        // Konstruktor publiczny do walidacji
        public DateDescriptionVO(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Opis daty nie może być pusty.", nameof(value));
            }
            if (value.Length > 100)
            {
                throw new ArgumentException("Opis daty nie może być dłuższy niż 100 znaków.", nameof(value));
            }
            this.Value = value;
        }

        // Prywatny konstruktor dla EF Core
        private DateDescriptionVO() { }

        // Konwersje
        public static implicit operator string(DateDescriptionVO desc) => desc.Value;
        public static implicit operator DateDescriptionVO(string text) => new(text);
    }
}