using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public class AddPhoneDto(Guid contactId, string number, Guid categoryId)
    {
        public Guid ContactId { get; } = contactId;
        public string Number { get; } = number;
        public Guid CategoryId { get; } = categoryId;
    }
}
