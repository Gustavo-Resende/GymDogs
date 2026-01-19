using GymDogs.Application.Users.Dtos;
using GymDogs.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymDogs.Application.Users.Extensions
{
    public static class UserExtensions
    {
        public static CreateUserDto ToCreateUserDto(this User user)
        {
            return new CreateUserDto(user.Id, user.Username, user.Email, user.CreatedAt);
        }
    }
}