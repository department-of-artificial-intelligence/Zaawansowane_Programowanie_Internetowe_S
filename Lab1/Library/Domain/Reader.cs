namespace Library.Domain;

public sealed class Reader(Guid id, string firstName, string lastName, string email)
{
    public Guid Id { get; } = id;
    public string FirstName { get; private set; } = firstName;
    public string LastName { get; private set; } = lastName;
    public string Email { get; private set; } = email;
    public bool Blocked { get; private set; }

    public void Block() => Blocked = true;
    public void Unblock() => Blocked = false;
}
