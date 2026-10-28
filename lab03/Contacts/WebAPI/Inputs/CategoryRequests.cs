namespace WebAPI.Inputs;

// Identyfikator właściciela nie jest częścią żądania - odczytujemy go z tokenu.
public class AddCategoryRequest
{
    public string Name {get; set;} = string.Empty;
}

public record NameRequest(string Name);
