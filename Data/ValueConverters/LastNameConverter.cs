using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Application.Domain;

namespace Contacts.Data.ValueConverters;

// Konwerter dla obiektu wartości LastName
public class LastNameConverter : ValueConverter<LastName, string>
{
    public LastNameConverter()
        // Konwersja z Value Object na DB type (string)
        : base(
            v => v.Value,
            // Konwersja z DB type (string) na Value Object. 
            // W tym momencie wywoływany jest konstruktor LastName i odbywa się WALIDACJA.
            v => new LastName(v))
    {
    }
}