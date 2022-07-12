using System.ComponentModel.DataAnnotations;

namespace SOCE.Library.Models.Accounts
{
    public class VerifyEmailRequest
    {
        [Required]
        public string Token { get; set; }
    }
}