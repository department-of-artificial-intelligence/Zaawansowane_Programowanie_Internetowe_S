using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Application.Domain;

namespace Contacts.Data.ValueConverters;

// Konwerter dla obiektu wartości FirstName
public class FirstNameConverter : ValueConverter<FirstName, string>
{
    public FirstNameConverter()
        // Konwersja z Value Object na DB type (string)
        : base(
            v => v.Value,
            // Konwersja z DB type (string) na Value Object. 
            // W tym momencie wywoływany jest konstruktor FirstName i odbywa się WALIDACJA.
            v => new FirstName(v))
    {
    }
}