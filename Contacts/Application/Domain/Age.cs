using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain
{
    public class Age
    {
        public int Value { get; }
        public Age(int value)
        {
            if(value < 18 && value < 120)
        }
    }
}