using System.Dynamic;

namespace Application.Utilities;

public record Unit
{
    private Unit() {}
    public static Unit Instace { get; } = new();

}
