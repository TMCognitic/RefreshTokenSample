using BStorm.Tools.CommandQuerySeparation.Queries;
using BStorm.Tools.CommandQuerySeparation.Results;
using RefreshTokenSample.Api.Models.Entities;

namespace RefreshTokenSample.Api.Models.Queries
{
    public class LoginQuery : IQueryDefinition<User>
    {
        public string Email { get; }
        public string Passwd { get; }

        public LoginQuery(string email, string passwd)
        {
            Email = email;
            Passwd = passwd;
        }
    }

    public class LoginQueryHandler : IQueryHandler<LoginQuery, User>
    {
        private readonly IList<User> _users;

        public LoginQueryHandler(IList<User> users)
        {
            _users = users;
        }

        public IQueryResult<User> Execute(LoginQuery query)
        {
            User? user = _users.SingleOrDefault(u =>  u.Email == query.Email && u.Passwd == query.Passwd);

            if (user is null)
                return IQueryResult<User>.Failure("Cet utilisateur n'existe pas", null);

            return IQueryResult<User>.Success(user);
        }
    }
}
