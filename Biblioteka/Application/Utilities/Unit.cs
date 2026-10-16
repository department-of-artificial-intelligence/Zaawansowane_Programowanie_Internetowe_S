using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utilities;
public record Unit
{
    private Unit() { }
    public static Unit Instance { get; } = new();
}

