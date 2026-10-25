using Application.Utilities;

namespace Application.Domain;

public class Reader : Entity
{
    public FullName Name {get;private set;}
    public EmailAddress Email {get; private set;}

    private Reader(Guid id, FullName name,EmailAddress email): base(id)
    {
        Name = name;
        Email = email;
    }
    private Reader(Guid id) : base(id)
    {
        Name = null!;
        Email = null!;
    }

    public static Result<Reader> Create(FullName name, EmailAddress email)
    {
        return Result.Success(new Reader(Guid.NewGuid(), name, email));
    }
    public Result<Unit> UpdateDetails(FullName fullName, EmailAddress email){
        Name = fullName;
        Email = email;
        return Result.Success(Unit.Instance);
    }
}