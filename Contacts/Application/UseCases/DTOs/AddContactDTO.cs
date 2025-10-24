using Contacts.Application.Domain; 
using System.Collections.Generic;

namespace Contacts.Application.UseCases.DTOs
{
    public class AddContactDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Sex Sex { get; set; }
        public int Age { get; set; }
        public List<AddContactEmailDTO> Emails { get; set; } = new List<AddContactEmailDTO>();
    }
}