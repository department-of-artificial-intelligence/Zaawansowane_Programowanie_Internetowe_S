using System;

namespace Contacts.Application.UseCases.DTOs
{
    public class AddImportantDateToContactDTO
    {
        public int ContactId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}