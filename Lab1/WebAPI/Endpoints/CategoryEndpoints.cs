using Application.Queries;
using Application.Queries.Dtos;
using Application.UseCases;
using Application.UseCases.Dtos;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Endpoints;

public static class CategoryEndpoints
{
    // Kod 500 (GlobalExceptionHandler) może zwrócić każda akcja, więc dokumentujemy go raz dla grupy.
    // ProducesProblem działa tylko na pojedynczej akcji, dlatego metadane dodajemy ręcznie.
    private static readonly ProducesResponseTypeMetadata InternalServerError = new(
        StatusCodes.Status500InternalServerError,
        typeof(ProblemDetails),
        ["application/problem+json"]);

    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        // Zadanie 4: wspólny prefiks ścieżki, wspólny tag i wspólny kod 500 dla wszystkich akcji grupy
        var group = app.MapGroup("/api/categories")
            .WithTags("Kategorie")
            .WithMetadata(InternalServerError);

        group.MapGet("/", (ICategoryQueries queries) =>
                Results.Ok(queries.GetCategories()))
            .WithName("GetCategories")
            .WithSummary("Zwraca wszystkie kategorie")
            .WithDescription("Zwraca listę wszystkich kategorii. Gdy nie ma żadnej, lista jest pusta.")
            .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", (Guid id, ICategoryQueries queries) =>
                queries.GetCategory(id).Match(
                    category => Results.Ok(category),
                    () => ResultHttpExtensions.NotFound(
                        $"Nie mogę odnaleźć kategorii o id = {id}")))
            .WithName("GetCategory")
            .WithSummary("Zwraca jedną kategorię")
            .WithDescription("Zwraca kategorię o podanym identyfikatorze.")
            .Produces<CategoryDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", (AddCategoryDto dto, ICategoryUseCases useCases) =>
                useCases.AddCategory(dto).ToHttpResult(category =>
                    Results.Created($"/api/categories/{category.Id}", category)))
            .WithName("AddCategory")
            .WithSummary("Dodaje kategorię")
            .WithDescription(
                "Tworzy nową kategorię i zwraca utworzony obiekt. " +
                "Nazwa nie może być pusta i musi być unikalna.")
            .Produces<AddCategoryResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPut("/{id:guid}", (Guid id, RenameCategoryRequest request, ICategoryUseCases useCases) =>
                useCases.RenameCategory(new RenameCategoryDto(id, request.Name)).ToNoContent())
            .WithName("RenameCategory")
            .WithSummary("Zmienia nazwę kategorii")
            .WithDescription(
                "Zmienia nazwę istniejącej kategorii. Nowa nazwa nie może być pusta " +
                "i nie może być zajęta przez inną kategorię.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", (Guid id, ICategoryUseCases useCases) =>
                useCases.RemoveCategory(id).ToNoContent())
            .WithName("RemoveCategory")
            .WithSummary("Usuwa kategorię")
            .WithDescription(
                "Usuwa kategorię. Kategorii przypisanej do jakiegokolwiek adresu e-mail " +
                "nie można usunąć (409).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
