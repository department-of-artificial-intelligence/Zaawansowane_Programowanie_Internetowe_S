

using Application.OutputPorts;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

public class UserServices(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserUseCases
{
    public Result<AddUserResult> AddUser(AddUserDto dto) => dto.ToDomain()
        .Tap(user => user.SetPassword(dto.Password))
        .Tap(userRepository.Add)
        .Tap(_ => unitOfWork.Save())
        .Map(AddUserResult.FromDomain);

}