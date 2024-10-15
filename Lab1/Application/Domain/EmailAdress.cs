using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain;

public record EmailAdress(string Adress)
{
    public static implicit operator string(EmailAdress adress)
        => adress.Adress;
    public static implicit operator EmailAdress(string text)
        => new (text);
    
}
