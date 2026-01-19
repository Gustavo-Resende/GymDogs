using System;
using System.Collections.Generic;
using System.Text;
using Ardalis.Specification;

namespace GymDogs.Domain.Users.Specification
{
    public class GetUserByUsernameSpec : Specification<User>
    {
        public GetUserByUsernameSpec(string username)
        {
            Query.Where(u => u.Username == username);
        }
    }
}
