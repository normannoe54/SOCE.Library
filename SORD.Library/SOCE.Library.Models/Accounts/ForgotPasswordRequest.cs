using System.ComponentModel.DataAnnotations;

namespace SOCE.Library.Models.Accounts
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}