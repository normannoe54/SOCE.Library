using System;
using System.Collections.Generic;
using System.Text;

namespace SORD.Library.Access.Models
{
    public class LoggedInUser
    {
        public string Token { get; set; }
        public string Id { get; set; }
        public string FirsName { get; set; }

        public string LastName { get; set; }

        public string EmailAdress { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
