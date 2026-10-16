using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Application.Utilities;

namespace Application.Domain;

public sealed record Isbn
{
    private static readonly Regex IsbnRegex = new(
        @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$",
        RegexOptions.Compiled);

    public string Value { get; }

    private Isbn(string value) => Value = value;

    public static Result<Isbn> Create(string value) =>
        Validators.NotEmpty(value, "Numer ISBN nie może być pusty.")
            .Bind(_ => IsbnRegex.IsMatch(value.Trim())
                ? Result.Success(new Isbn(value.Trim().Replace(" ", "")))
                : Result.Failure<Isbn>($"Niepoprawny format ISBN: '{value}'."));
}
