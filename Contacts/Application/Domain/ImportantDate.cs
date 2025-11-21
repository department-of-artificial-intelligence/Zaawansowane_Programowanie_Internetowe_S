using System;

namespace Contacts.Application.Domain
{
    public class ImportantDate
    {
        public int Id { get; private set; }
        
        // Data sama w sobie jest obiektem wartości, więc używamy DateTime
        public DateTime Date { get; private set; }
        
        // Używamy naszego nowego obiektu wartości dla opisu
        public DateDescriptionVO Description { get; private set; } = null!;
        
        // Relacja do Kontaktu
        public Contact Contact { get; private set; } = null!;
        public int ContactId { get; private set; }

        // Konstruktor dla EF Core
        private ImportantDate() { }

        // Konstruktor publiczny do tworzenia
        public ImportantDate(DateTime date, DateDescriptionVO description)
        {
            // Możemy tu dodać walidację, np. czy data nie jest z przyszłości
            Date = date;
            Description = description;
        }
    }
}