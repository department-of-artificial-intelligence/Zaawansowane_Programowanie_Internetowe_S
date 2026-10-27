using Application.Domain;

namespace Application.UseCases.Dtos;

public record UpdateContactDto(
    string FirstName,
    string LastName,
    Sex Sex,
    int Age
);