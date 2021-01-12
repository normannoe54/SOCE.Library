using System.ComponentModel.DataAnnotations;

namespace SORD.Library.Models.Accounts
{
    public class ValidateResetTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}