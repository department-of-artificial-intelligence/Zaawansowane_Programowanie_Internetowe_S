namespace Library.Application.Utilities;

public record Unit
{
    private Unit() { }
    public static Unit Instance { get; } = new();
}