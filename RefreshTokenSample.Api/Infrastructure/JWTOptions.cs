namespace RefreshTokenSample.Api.Infrastructure
{
    public class JWTOptions
    {
        public string SecretKey { get; }
        public string Issuer { get; }
        public string Audience { get; }
        public int DurationInMins { get; }
        public JWTOptions(string secretKey, string issuer, string audience, int durationInMins)
        {
            SecretKey = secretKey;
            Issuer = issuer;
            Audience = audience;
            DurationInMins = durationInMins;
        }
    }
}
