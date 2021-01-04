using System;
using System.Collections.Generic;
using System.Text;

namespace SORD.Library.Kernel.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FirsName { get; set; }

        public string LastName { get; set; }

        public string EmailAdress { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
