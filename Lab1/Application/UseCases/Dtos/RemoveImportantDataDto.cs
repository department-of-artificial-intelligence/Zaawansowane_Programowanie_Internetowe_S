using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class RemoveImportantDateDto(Guid contactId, string description)
    {
        public Guid ContactId { get; } = contactId;
        public string Description { get; } = description;
    }
}
