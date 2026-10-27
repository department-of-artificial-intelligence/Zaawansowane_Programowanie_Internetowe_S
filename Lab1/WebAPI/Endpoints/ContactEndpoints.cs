using Application.Queries;
using Application.Queries.Dtos;
using Application.UseCases;
using Application.UseCases.Dtos;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Endpoints;

public static class ContactEndpoints
{
    // Kod 500 (GlobalExceptionHandler) może zwrócić każda akcja, więc dokumentujemy go raz dla grupy.
    // ProducesProblem działa tylko na pojedynczej akcji, dlatego metadane dodajemy ręcznie.
    private static readonly ProducesResponseTypeMetadata InternalServerError = new(
        StatusCodes.Status500InternalServerError,
        typeof(ProblemDetails),
        ["application/problem+json"]);

    public static RouteGroupBuilder MapContactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contacts")
            .WithTags("Kontakty")
            .WithMetadata(InternalServerError);

        // ---------- Kontakt ----------

        group.MapGet("/", (IContactQueries queries) =>
                Results.Ok(queries.GetContacts()))
            .WithName("GetContacts")
            .WithSummary("Zwraca wszystkie kontakty")
            .WithDescription("Zwraca listę kontaktów (bez e-maili, telefonów i dat).")
            .Produces<IEnumerable<ContactSummaryDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", (Guid id, IContactQueries queries) =>
                queries.GetContact(id).Match(
                    contact => Results.Ok(contact),
                    () => ResultHttpExtensions.NotFound($"Brak kontaktu (id: {id})")))
            .WithName("GetContact")
            .WithSummary("Zwraca jeden kontakt")
            .WithDescription("Zwraca kontakt razem z adresami e-mail, numerami telefonów i ważnymi datami.")
            .Produces<ContactDetailsDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", (AddContactDto dto, IContactUseCases useCases) =>
                useCases.AddContact(dto).ToHttpResult(contact =>
                    Results.Created($"/api/contacts/{contact.Id}", contact)))
            .WithName("AddContact")
            .WithSummary("Dodaje kontakt")
            .WithDescription(
                "Tworzy nowy kontakt. Imię i nazwisko są wymagane (maks. 50 znaków), " +
                "wiek musi mieścić się w dozwolonym zakresie.")
            .Produces<AddContactResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", (Guid id, UpdateContactRequest request, IContactUseCases useCases) =>
                useCases.UpdateContact(new UpdateContactDto(
                        id, request.FirstName, request.LastName, request.Sex, request.Age))
                    .ToNoContent())
            .WithName("UpdateContact")
            .WithSummary("Edytuje kontakt")
            .WithDescription(
                "Zmienia imię, nazwisko, płeć i wiek kontaktu. " +
                "Gdy któraś wartość jest niepoprawna, kontakt nie zmienia się wcale.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", (Guid id, IContactUseCases useCases) =>
                useCases.RemoveContact(id).ToNoContent())
            .WithName("RemoveContact")
            .WithSummary("Usuwa kontakt")
            .WithDescription("Usuwa kontakt razem z jego adresami e-mail, telefonami i datami.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // ---------- E-maile ----------

        group.MapPost("/{id:guid}/emails", (Guid id, AddEmailRequest request, IContactUseCases useCases) =>
                useCases.AddEmail(new AddEmailDto(id, request.Address, request.CategoryId))
                    .ToNoContent())
            .WithName("AddEmail")
            .WithSummary("Dodaje adres e-mail do kontaktu")
            .WithDescription(
                "Dołącza adres e-mail z podaną kategorią. Adres musi mieć poprawny format, " +
                "kategoria musi istnieć, a kontakt nie może mieć już tego adresu.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}/emails/{address}", (Guid id, string address, IContactUseCases useCases) =>
                useCases.RemoveEmail(new RemoveEmailDto(id, address)).ToNoContent())
            .WithName("RemoveEmail")
            .WithSummary("Usuwa adres e-mail z kontaktu")
            .WithDescription("Usuwa podany adres e-mail. Wielkość liter w adresie nie ma znaczenia.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // ---------- Telefony ----------

        group.MapPost("/{id:guid}/phones", (Guid id, AddPhoneRequest request, IContactUseCases useCases) =>
                useCases.AddPhone(new AddPhoneDto(id, request.Number, request.Type)).ToNoContent())
            .WithName("AddPhone")
            .WithSummary("Dodaje numer telefonu do kontaktu")
            .WithDescription(
                "Dołącza numer telefonu (9–15 cyfr, opcjonalnie z \"+\"; spacje, myślniki " +
                "i nawiasy są pomijane). Ten sam numer nie może wystąpić w kontakcie dwa razy.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}/phones/{number}", (Guid id, string number, IContactUseCases useCases) =>
                useCases.RemovePhone(new RemovePhoneDto(id, number)).ToNoContent())
            .WithName("RemovePhone")
            .WithSummary("Usuwa numer telefonu z kontaktu")
            .WithDescription("Usuwa podany numer telefonu z kontaktu.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        // ---------- Ważne daty ----------

        group.MapPost("/{id:guid}/dates", (Guid id, ImportantDateRequest request, IContactUseCases useCases) =>
                useCases.AddImportantDate(new AddImportantDateDto(id, request.Label, request.Date))
                    .ToHttpResult(dateId => Results.Created(
                        $"/api/contacts/{id}", new CreatedIdResponse(dateId))))
            .WithName("AddImportantDate")
            .WithSummary("Dodaje ważną datę do kontaktu")
            .WithDescription(
                "Dodaje datę z etykietą (np. \"Urodziny\") i zwraca jej identyfikator. " +
                "Etykiety w obrębie kontaktu są unikalne.")
            .Produces<CreatedIdResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}/dates/{dateId:guid}",
                (Guid id, Guid dateId, ImportantDateRequest request, IContactUseCases useCases) =>
                    useCases.ChangeImportantDate(
                            new ChangeImportantDateDto(id, dateId, request.Label, request.Date))
                        .ToNoContent())
            .WithName("ChangeImportantDate")
            .WithSummary("Zmienia ważną datę kontaktu")
            .WithDescription("Zmienia etykietę i datę. Nowa etykieta nie może być zajęta przez inną datę kontaktu.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}/dates/{dateId:guid}", (Guid id, Guid dateId, IContactUseCases useCases) =>
                useCases.RemoveImportantDate(new RemoveImportantDateDto(id, dateId)).ToNoContent())
            .WithName("RemoveImportantDate")
            .WithSummary("Usuwa ważną datę z kontaktu")
            .WithDescription("Usuwa datę o podanym identyfikatorze.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
