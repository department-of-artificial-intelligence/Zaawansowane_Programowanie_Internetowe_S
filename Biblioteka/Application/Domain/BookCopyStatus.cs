using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public enum BookCopyStatus
    {
        Available = 1,
        Borrowed = 2,
        Reserved = 3,
        Withdrawn = 4
    }
}
