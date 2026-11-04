using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain;

public sealed class LastName
{
    public string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public static Result<LastName> Create(string value)
    {
        return Validators.NotEmpty(value, "Nazwisko jest wymagane.")
            .Map(_ => new LastName(value.Trim()));
    }
}
