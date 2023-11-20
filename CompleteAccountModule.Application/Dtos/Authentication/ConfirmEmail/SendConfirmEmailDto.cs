using System.ComponentModel.DataAnnotations;

namespace CompleteAccountModule.Application.Dtos.Authentication.ConfirmEmail
{
    public class SendConfirmEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
