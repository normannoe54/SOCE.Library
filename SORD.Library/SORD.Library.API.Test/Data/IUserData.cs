using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SORD.Library.API.Test.Models;

namespace SORD.Library.API.Test.Data
{
    public interface IUserData
    {
        bool SaveChanges();
        IEnumerable<User> GetUsers();
        User GetUserById(int id);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);

    }
}
