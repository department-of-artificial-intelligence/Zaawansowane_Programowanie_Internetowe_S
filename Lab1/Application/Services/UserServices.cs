using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Services
{
    public class UserServices(
     IUserRepository userRepository,
     ITokenGenerator tokenGenerator,
     IUnitOfWork unitOfWork,
     UserUniquenessChecker userUniquenessChecker,
     IPasswordHasher passwordHasher // <-- 1. Wstrzyknięcie hashera
 ) : IUserUseCases
    {
        public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
            addUserDto
                .ToDomain()
                .Bind(user => userUniquenessChecker
                    .EnsureUnique(user.UserName)
                    .Map(_ => user))
                // 2. Obliczamy skrót hasła przed zapisem do bazy
                .Tap(user => user.SetPassword(passwordHasher.HashPassword(addUserDto.Password)))
                .Tap(userRepository.Add)
                .Tap(_ => unitOfWork.Save())
                .Map(AddUserResult.FromDomain);

        public Result<LoginUserResult> Login(LoginUserDto dto) =>
            userRepository.GetByName(dto.UserName)
                .Required("Błędna nazwa użytkownika")
                .Bind(user => passwordHasher.VerifyHashedPassword(user.Password, dto.Password)
                    ? Result.Success(user)
                    : Result.Failure<User>("Błędne hasło"))
                .Tap(user =>
                {
                    var refreshToken = tokenGenerator.GenerateRefreshToken();
                    user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
                })
                .Tap(_ => unitOfWork.Save())
                .Map(user => new LoginUserResult(
                    tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString()),
                    user.RefreshToken!));

        public Result<LoginUserResult> RefreshToken(RefreshTokenDto dto) =>
            userRepository.GetByRefreshToken(dto.RefreshToken)
                .Required("Nieprawidłowy refresh token")
                .Bind(user => user.RefreshTokenExpiryTime > DateTime.UtcNow
                    ? Result.Success(user)
                    : Result.Failure<User>("Refresh token wygasł"))
                .Tap(user =>
                {
                    // Rotacja tokenu: przy każdym odświeżeniu wydajemy nowy refresh token
                    var newRefreshToken = tokenGenerator.GenerateRefreshToken();
                    user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
                })
                .Tap(_ => unitOfWork.Save())
                .Map(user => new LoginUserResult(
                    tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString()),
                    user.RefreshToken!));

        public Result<Unit> UpdateProfile(UpdateUserProfileDto dto) =>
            userRepository.GetById(dto.UserId)
                .Required("Nie znaleziono użytkownika")
                .Tap(user => user.UpdateProfile(dto.FirstName, dto.LastName, dto.DateOfBirth, dto.Address))
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance);

        public Result<Unit> RemoveUser(Guid id) =>
            userRepository.GetById(id)
                .Required("Nie znaleziono użytkownika")
                .Tap(userRepository.Remove)
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance);

        public Result<Unit> ChangeRole(ChangeUserRoleDto dto) =>
            userRepository.GetById(dto.UserId)
                .Required("Nie znaleziono użytkownika")
                .Tap(user => user.ChangeRole(dto.NewRole))
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance);
    }
}



