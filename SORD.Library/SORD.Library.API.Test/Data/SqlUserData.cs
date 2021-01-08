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

        public void CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.users.Add(user);
        }



        public User GetUserById(int id)
        {
            return _context.users.FirstOrDefault<User>(x=> x.Id == id);
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.users.ToList();
        }

        public bool SaveChanges()
        {
            //0 is success?
            bool ret = _context.SaveChanges() >= 0;

            return ret;
        }

        public void UpdateUser(User user)
        {
            //return nothing
        }

        public void DeleteCommand(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _context.users.Remove(user);
        }
    }
}
