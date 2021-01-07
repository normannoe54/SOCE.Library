using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORD.Library.API.Test.Dtos
{
    public class UserReadDto
    {
        public string Token { get; set; }

        public int Id { get; set; }

        public string FirsName { get; set; }

        public string LastName { get; set; }

        public string EmailAdress { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
