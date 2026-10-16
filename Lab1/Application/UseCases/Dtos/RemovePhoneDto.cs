using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class RemovePhoneDto(Guid contactId, string number)
    {
        public Guid ContactId { get; } = contactId;
        public string Number { get; } = number;
    }
}
