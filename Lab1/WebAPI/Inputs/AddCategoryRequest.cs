namespace WebAPI.Inputs;

// 3.30: UserId nie przychodzi od klienta, tylko z tokenu (claim "sub")
public class AddCategoryRequest
{
    public string Name { get; set; } = string.Empty;
}
