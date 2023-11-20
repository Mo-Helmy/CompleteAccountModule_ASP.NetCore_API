using System.ComponentModel.DataAnnotations;

namespace CompleteAccountModule.Application.Dtos.Authentication
{
    public class UpdateUserDetailsDto
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
