namespace Contacts.Application.Domain
{
    public class Phone
    {
        public int Id { get; private set; }
        
        // U偶ywamy obiektu warto艣ci
        public PhoneNumberVO Number { get; private set; } = null!;
        
        // Relacja do Kontaktu
        public Contact Contact { get; private set; } = null!;
        public int ContactId { get; private set; }

        // Relacja do Kategorii
        public int CategoryId { get; private set; }
        public Category Category { get; private set; } = null!;

        // Konstruktor dla EF Core
        private Phone() { }

        // Konstruktor publiczny do tworzenia
        public Phone(PhoneNumberVO number, int categoryId)
        {
            Number = number;
            CategoryId = categoryId;
        }
    }
}