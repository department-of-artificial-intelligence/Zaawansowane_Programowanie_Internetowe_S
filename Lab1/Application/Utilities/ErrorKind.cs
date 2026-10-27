namespace Application.Utilities;

/// <summary>
/// Rodzaj błędu. Warstwa WebAPI tłumaczy go na kod HTTP:
/// Validation -> 400, NotFound -> 404, Conflict -> 409.
/// </summary>
public enum ErrorKind
{
    Validation,
    NotFound,
    Conflict
}
