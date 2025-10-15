namespace Contacts.Application.Domain
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        // Konstruktor, który umożliwia ustawienie nazwy i opcjonalnie id
        public Category(string name, int id)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        // Konstruktor przeciążony, który ustawia id na 0, jeśli nie jest podane
        public Category(string name) : this(name, 0) { }
    }
}
