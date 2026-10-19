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
Validators.NotEmpty(name, "Nazwa kategorii nie może być pusta.")
.Tap(_ => Name = name)
.Map(_ => Unit.Instance);
public static Result<Category> Create(string name) =>
Validators
.NotEmpty(name, "Nazwa kategorii nie może być pusta.")
.Map(_ => new Category(Guid.NewGuid(), name));
}