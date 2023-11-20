using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CompleteAccountModule.Application.Dtos.Authentication.ForgetPassword
{
    public class ForgetPasswordDto
    {
        [FromRoute]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [FromBody]
        [Required]
        public string Token { get; set; }

        [FromBody]
        [Required]
        public string NewPassword { get; set; }
    }
}
