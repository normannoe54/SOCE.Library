using Microsoft.EntityFrameworkCore;
using SORD.Library.API.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORD.Library.API.Test.Data
{
    public class UserContext:DbContext
    {
        public DbSet<User> users { get; set; }
        public UserContext(DbContextOptions<UserContext> userContext) : base(userContext)
        {

        }

    }
}
