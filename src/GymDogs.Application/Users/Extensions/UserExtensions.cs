using GymDogs.Application.Users.Dtos;
using GymDogs.Domain.Users;

namespace GymDogs.Application.Users.Extensions;

public static class UserExtensions
{
    public static CreateUserDto ToCreateUserDto(this User user)
    {
        return new CreateUserDto(user.Id, user.Username, user.Email, user.CreatedAt);
    }

    public static GetUserDto ToGetUserDto(this User user)
    {
        return new GetUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.CreatedAt,
            user.LastUpdatedAt);
    }
}