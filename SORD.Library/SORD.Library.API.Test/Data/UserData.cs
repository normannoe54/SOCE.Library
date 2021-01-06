using SORD.Library.API.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORD.Library.API.Test.Data
{
    public class UserData : IUserData
    {
        public User GetUserById(int id)
        {
            User user = new User();
            user.Id = 0;
            return user;         
        }

        public IEnumerable<User> GetUsers()
        {
            User user1 = new User() { Id = 1 };
            List<User> list = new List<User>() { user1 };
            return list;
        }
    }
}
