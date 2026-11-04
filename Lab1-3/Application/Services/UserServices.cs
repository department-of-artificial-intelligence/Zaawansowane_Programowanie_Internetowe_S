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

namespace Application.Services;

public class UserServices(
    IUserRepository userRepository,
    ITokenGenerator tokenGenerator,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    UserUniquenessChecker userUniquenessChecker
) : IUserUseCases
{
    public Result<AddUserResult> AddUser(AddUserDto addUserDto) =>
        userUniquenessChecker.EnsureUnique(addUserDto.UserName)
            .Bind(_ => addUserDto.ToDomain())
            .Tap(user => user.SetPassword(passwordHasher.HashPassword(addUserDto.Password)))
            .Tap(userRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(AddUserResult.FromDomain);

    public Result<LoginUserResult> Login(LoginUserDto dto)
        => userRepository.GetByName(dto.UserName)
        .Required("Błędna nazwa użytkownika")
        .Bind(user =>
        {
            if (!passwordHasher.VerifyPassword(user.Password, dto.Password))
            {
                return Result.Failure<LoginUserResult>("Błędne hasło");
            }

            var accessToken = tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString());
            var refreshToken = tokenGenerator.GenerateRefreshToken();

            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7)); // Refresh token ważny np. 7 dni
            unitOfWork.Save();

            return Result.Success(new LoginUserResult(accessToken, refreshToken));
        });

    public Result<LoginUserResult> RefreshToken(RefreshTokenDto dto)
        => userRepository.GetByName(dto.UserName)
        .Required("Nie znaleziono użytkownika")
        .Bind(user =>
        {
            if (!user.IsRefreshTokenValid(dto.RefreshToken))
            {
                return Result.Failure<LoginUserResult>("Nieprawidłowy lub wygasły Refresh Token");
            }

            var newAccessToken = tokenGenerator.GenerateToken(user.Id, user.UserName, user.Role.ToString());
            var newRefreshToken = tokenGenerator.GenerateRefreshToken();

            user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
            unitOfWork.Save();

            return Result.Success(new LoginUserResult(newAccessToken, newRefreshToken));
        });

    public Result<Unit> RemoveUser(Guid id) =>
        userRepository.GetById(id)
            .Required("Nie znaleziono użytkownika")
            .Tap(userRepository.Remove)
            .Tap(_ => unitOfWork.Save())
            .Map(_ => (Unit)default!);

    public IEnumerable<UserDto> GetUsers() =>
        userRepository.GetAll().Select(UserDto.FromDomain);

    public Result<Unit> UpdateProfile(Guid userId, UserProfileDto profileDto) =>
        userRepository.GetById(userId)
            .Required("Nie znaleziono użytkownika")
            .Bind(user => profileDto.ToDomain().Map(profile => (user, profile)))
            .Tap(pair => pair.user.UpdateProfile(pair.profile))
            .Tap(_ => unitOfWork.Save())
            .Map(_ => (Unit)default!);
    public Result<Unit> ChangeRole(Guid userId, UserRole newRole) =>
    userRepository.GetById(userId)
        .Required("Nie znaleziono użytkownika")
        .Tap(user => user.ChangeRole(newRole))
        .Tap(_ => unitOfWork.Save())
        .Map(_ => (Unit)default!);

}

