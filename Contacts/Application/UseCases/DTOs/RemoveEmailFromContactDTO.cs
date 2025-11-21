namespace Contacts.Application.UseCases.DTOs
{
    // DTO do przenoszenia danych dla operacji usunięcia emaila
    public class RemoveEmailFromContactDTO
    {
        // Z którego kontaktu usuwamy?
        public int ContactId { get; set; }
        
        // Który email usuwamy?
        public int EmailId { get; set; }
    }
}