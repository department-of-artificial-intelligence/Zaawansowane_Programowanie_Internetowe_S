namespace Application.OutputPorts;

public interface IClock
{
    DateTime UtcNow { get; }
    DateOnly Today => DateOnly.FromDateTime(UtcNow);
}
