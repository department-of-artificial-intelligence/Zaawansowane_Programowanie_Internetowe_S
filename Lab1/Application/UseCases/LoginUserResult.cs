namespace Application.UseCases.Dtos;

public class LoginUserResult(string accessToken = ""){
	public string AccessToken { get; } = accessToken;
}
