using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain;

public sealed class FirstName
{
    public string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public static Result<FirstName> Create(string value)
    {
        return Validators.NotEmpty(value, "Imię jest wymagane.")
            .Map(_ => new FirstName(value.Trim()));
    }
}
