using System;
using System.Text.RegularExpressions;

namespace Contacts.Application.Domain
{
    // Obiekt warto艣ci dla numeru telefonu
    public record PhoneNumberVO
    {
        // Prosty regex dla 9-cyfrowego numeru, dla uproszczenia
        private static readonly Regex PhoneRegex = new(
            @"^[0-9]{9}$", 
            RegexOptions.Compiled);

        public string Number { get; private set; } = null!; // null! dla EF Core i warningu

        // Konstruktor publiczny do walidacji
        public PhoneNumberVO(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Numer telefonu nie mo偶e by膰 pusty.", nameof(number));
            }
            
            // Uproszczona walidacja - mo偶na j膮 rozbudowa膰
            if (!PhoneRegex.IsMatch(number.Replace("-", "").Replace(" ", "")))
            {
                throw new ArgumentException("Niepoprawny format numeru telefonu (wymagane 9 cyfr).", nameof(number));
            }

            this.Number = number;
        }

        // Prywatny konstruktor dla EF Core
        private PhoneNumberVO() { }

        // Konwersje u艂atwiaj膮ce prac臋
        public static implicit operator string(PhoneNumberVO phone) => phone.Number;
        public static implicit operator PhoneNumberVO(string text) => new(text);
    }
}