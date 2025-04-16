using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace RefreshTokenSample.Api.Models.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(75, MinimumLength = 1)]
        public required string Nom { get; set; }
        [Required]
        [StringLength(75, MinimumLength = 1)]
        public required string Prenom { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public required string Passwd { get; set; }
    }
}
