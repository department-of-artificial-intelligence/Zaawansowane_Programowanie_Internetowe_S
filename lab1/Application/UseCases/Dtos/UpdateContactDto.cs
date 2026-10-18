
using Application.Domain;

namespace Application.UseCases.Dtos;

public record UpdateContactDto(
    Guid Id,
    string FirstName,
    string LastName,
    Sex Sex,
    int Age
);
