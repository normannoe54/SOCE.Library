using System.ComponentModel.DataAnnotations;

namespace SOCE.Library.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}