using Application.Utilities;

namespace Application.Domain;

public sealed record Author
{
    public FullName Name {get;}

    private Author(FullName name)
    {
        Name = name;
    }
    private Author()
    {
        Name = null!;
    }
    public static Result<Author> Create(FullName name)
    {
        return Result.Success(new Author(name));
    }
}