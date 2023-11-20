#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CompleteAccountModule.Application.Dtos.Authentication
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
