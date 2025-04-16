namespace RefreshTokenSample.Api.Models.Entities
{
    public class User
    {
        public Guid Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }
        public string Passwd { get; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }

        public User(Guid id, string nom, string prenom, string email, string passwd)
        {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Passwd = passwd;
        }
    }
}
