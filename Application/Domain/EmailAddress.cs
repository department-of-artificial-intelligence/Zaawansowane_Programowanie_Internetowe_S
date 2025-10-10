using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Domain;

public record EmailAddress(string Address)
{
    public static implicit operator string(EmailAddress address) => address.Address;

    public static implicit operator EmailAddress(string text) => new(text);
}