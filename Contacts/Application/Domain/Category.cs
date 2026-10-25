using Application.Utilities;
namespace Application.Domain;

public class Category : Entity
{
    public string Name { get; private set; }
    private Category(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public Result<Unit> Rename(string name) =>
        Validators.NotEmpty(name, "Nazwa kategorii nie moze byc pusta.")
        .Tap(_ => Name = name)
        .Map(_ => Unit.Instance);

    public static Result<Category> Create(string name) =>
        Validators
            .NotEmpty(name, "Nazwa kategorii nie moze byc pusta.")
            .Map(_ => new Category(Guid.NewGuid(), name));

    public Result<Unit> UpdateName(string newName)
    {
        return Validators.NotEmpty(newName, "Nowa nazwa kategorii jest wymagana.")
        .Tap(_ => Name = newName)
        .Map(_ => Unit.Instance);
    }
}