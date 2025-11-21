using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain;

public record Age
{
    public int Value { get; }
    public Age(int value)
    {
        // POPRAWKA: Musi być || (LUB)
        if (value < 18 || value > 120)
            throw new ArgumentOutOfRangeException(nameof(value), "Wiek musi być pomiędzy 18 a 120 lat.");
        Value = value;
    }
    public static implicit operator int(Age age) => age.Value;
    public static implicit operator Age(int value) => new(value);
}