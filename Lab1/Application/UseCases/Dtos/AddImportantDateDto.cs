using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class AddImportantDateDto(Guid contactId, DateOnly date, string description)
    {
        public Guid ContactId { get; } = contactId;
        public DateOnly Date { get; } = date;
        public string Description { get; } = description;
    }
}
