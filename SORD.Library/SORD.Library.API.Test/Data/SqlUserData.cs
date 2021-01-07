using SORD.Library.API.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORD.Library.API.Test.Data
{
    public class SqlUserData : IUserData
    {
        private readonly UserContext _context;

        public SqlUserData(UserContext context)
        {
            _context = context;
        }

        public User GetUserById(int id)
        {
            return _context.users.FirstOrDefault<User>(x=> x.Id == id);
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.users.ToList();
        }
    }
}
