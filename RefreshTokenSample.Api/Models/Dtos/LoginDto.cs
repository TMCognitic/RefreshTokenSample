using System.ComponentModel.DataAnnotations;

namespace RefreshTokenSample.Api.Models.Dtos
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public required string Passwd { get; set; }
    }
}
