using System;

namespace SOCE.Library.Models.Accounts
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsVerified { get; set; }
    }
}