using System.ComponentModel.DataAnnotations;

namespace RefreshTokenSample.Api.Models.Dtos
{
    public class TokensDto
    {
        [Required]
        public required string Token { get; set; }
        [Required]
        public required string RefreshToken { get; set; }
    }
}
