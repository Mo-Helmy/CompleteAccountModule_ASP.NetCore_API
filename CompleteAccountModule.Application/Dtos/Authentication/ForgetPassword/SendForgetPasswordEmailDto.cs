using System.ComponentModel.DataAnnotations;

namespace CompleteAccountModule.Application.Dtos.Authentication.ForgetPassword
{
    public class SendForgetPasswordEmailDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
