using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.Models.Accounts
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public RegisterEnum register { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
    }
}
