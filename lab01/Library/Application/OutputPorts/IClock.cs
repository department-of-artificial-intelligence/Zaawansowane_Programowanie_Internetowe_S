namespace Application.OutputPorts;

// Port wyjściowy dla aktualnego czasu - dzięki niemu reguły zależne od dat
// (terminy zwrotu, przeterminowanie) można testować bez zależności od zegara systemowego.
public interface IClock
{
    DateTime Now { get; }
    DateOnly Today => DateOnly.FromDateTime(Now);
}
