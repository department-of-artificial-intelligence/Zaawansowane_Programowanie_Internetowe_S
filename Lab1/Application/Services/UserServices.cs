using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class UserServcies(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserUseCases
{
    public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        addUserDto.ToDomain()
            .Tap(user => user.SetPassword(addUserDto.Password))
            .Tap(userRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddUserResult.FromDomain);
}