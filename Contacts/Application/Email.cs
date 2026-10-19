using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Utilities;

namespace Application.Domain;

public sealed record Email
{
    public EmailAddress Address{get;}
    public Guid CategoryId{get;}

    private Email (EmailAddress address, Guid categoryId)
    {
        Address = address;
        CategoryId = categoryId;

    }

    internal static Result<Email> Create(
        EmailAddress address,
        Guid categoryId) =>
        Result.Success(new Email(address, categoryId));

}