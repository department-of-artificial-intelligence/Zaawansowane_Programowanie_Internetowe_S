using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos;

public class RemoveEmailDto(Guid contactId, string address)
{
    public Guid ContactId { get; } = contactId;
    public string Address { get; } = address;
}
