using System.ComponentModel.DataAnnotations;

namespace SORD.Library.Models.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}