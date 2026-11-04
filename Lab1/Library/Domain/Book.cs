namespace Library.Domain;

public sealed class Book(Guid id, Isbn isbn, string title, string author)
{
    public Guid Id { get; } = id;
    public Isbn Isbn { get; } = isbn;
    public string Title { get; private set; } = title;
    public string Author { get; private set; } = author;
    public bool IsAvailable { get; private set; } = true;

    public void MarkBorrowed() => IsAvailable = false;
    public void MarkReturned() => IsAvailable = true;
}
