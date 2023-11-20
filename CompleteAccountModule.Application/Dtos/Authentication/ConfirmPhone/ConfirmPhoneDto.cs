using System.ComponentModel.DataAnnotations;

namespace CompleteAccountModule.Application.Dtos.Authentication.ConfirmPhone
{
    public class ConfirmPhoneDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
