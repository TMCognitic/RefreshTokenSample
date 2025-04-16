using BStorm.Tools.CommandQuerySeparation.Queries;
using BStorm.Tools.CommandQuerySeparation.Results;
using RefreshTokenSample.Api.Models.Entities;

namespace RefreshTokenSample.Api.Models.Queries
{
    public class GetUserByTokensQuery : IQueryDefinition<User>
    {
        public string Token { get; }
        public string RefreshToken { get; }
        public GetUserByTokensQuery(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }

    public class GetUserByTokensQueryHandler : IQueryHandler<GetUserByTokensQuery, User>
    {
        private readonly IList<User> _users;

        public GetUserByTokensQueryHandler(IList<User> users)
        {
            _users = users;
        }

        public IQueryResult<User> Execute(GetUserByTokensQuery query)
        {
            User? user = _users.SingleOrDefault(u => u.Token == query.Token && u.RefreshToken == query.RefreshToken);

            if (user is null)
                return IQueryResult<User>.Failure("Cet utilisateur n'existe pas", null);

            return IQueryResult<User>.Success(user);
        }
    }
}
