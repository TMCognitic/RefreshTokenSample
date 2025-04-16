using BStorm.Tools.CommandQuerySeparation.Commands;
using BStorm.Tools.CommandQuerySeparation.Results;
using RefreshTokenSample.Api.Models.Entities;

namespace RefreshTokenSample.Api.Models.Commands
{
    public class RegisterCommand : ICommandDefinition
    {
        public Guid Id { get; }
        public string Nom { get; }
        public string Prenom { get; }
        public string Email { get; }
        public string Passwd { get; }

        public RegisterCommand(string nom, string prenom, string email, string passwd)
        {
            Id = Guid.NewGuid();
            Nom = nom;
            Prenom = prenom;
            Email = email;
            Passwd = passwd;
        }
    }

    public class RegisterCommandHandler : ICommandHandler<RegisterCommand>
    {
        private readonly IList<User> _users;
        public RegisterCommandHandler(IList<User> users)
        {
            _users = users;
        }

        public ICommandResult Execute(RegisterCommand command)
        {
            if (_users.Any(u => u.Email == command.Email))
                return ICommandResult.Failure("Cet email existe déjà");

            _users.Add(new User(command.Id, command.Nom, command.Prenom, command.Email, command.Passwd));
            return ICommandResult.Success();
        }
    }
}
