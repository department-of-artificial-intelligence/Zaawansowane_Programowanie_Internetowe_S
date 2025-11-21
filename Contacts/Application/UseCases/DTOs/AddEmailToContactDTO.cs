namespace Contacts.Application.UseCases.DTOs
{
    // DTO do przenoszenia danych dla operacji dodania emaila
    public class AddEmailToContactDTO
    {
        // Do którego kontaktu chcemy dodać?
        public int ContactId { get; set; }
        
        // Jaki adres email?
        public string Address { get; set; } = string.Empty;
        
        // Do jakiej kategorii należy?
        public int CategoryId { get; set; }
    }
}