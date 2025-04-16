using System.Security.Cryptography;
using System.Text;

namespace RefreshTokenSample.Api.Infrastructure
{
    public static class StringExtensions
    {
        public static string Hash(this string value)
        {
            return Convert.ToBase64String(SHA512.HashData(Encoding.Default.GetBytes(value)));
        }
    }
}
