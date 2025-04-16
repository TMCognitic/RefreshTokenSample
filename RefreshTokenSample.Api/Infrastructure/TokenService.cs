using Microsoft.IdentityModel.Tokens;
using RefreshTokenSample.Api.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RefreshTokenSample.Api.Infrastructure
{
    public class TokenService : ITokenService
    {
        private readonly JWTOptions _options;

        public TokenService(JWTOptions options)
        {
            _options = options;
        }

        public void GenerateToken(User user)
        {
            byte[] secretKey = Encoding.Default.GetBytes(_options.SecretKey);
            SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(secretKey);


            //Définition des claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.Prenom} {user.Nom}")
            };

            //Génération du token
            JwtSecurityToken Token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.DurationInMins),
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
             );

            string token = new JwtSecurityTokenHandler().WriteToken(Token);
            //Ajout du token et du refresh
            user.Token = token;
            user.RefreshToken = GenerateRefreshToken();
        }

        public string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[64];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public TokenPair RefreshTokenPair(string oldToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = handler.ReadToken(oldToken) as JwtSecurityToken;

            if (jwtToken is null)
                throw new InvalidOperationException("Invalid Token");

            byte[] secretKey = Encoding.Default.GetBytes(_options.SecretKey);
            SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(secretKey);

            IEnumerable<Claim> claims = jwtToken.Claims;

            JwtSecurityToken Token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_options.DurationInMins),
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
             );

            string newToken = new JwtSecurityTokenHandler().WriteToken(Token);
            return new TokenPair(newToken, GenerateRefreshToken());
        }
    }
}
