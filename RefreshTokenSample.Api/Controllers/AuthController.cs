using BStorm.Tools.CommandQuerySeparation.Dispatching;
using BStorm.Tools.CommandQuerySeparation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RefreshTokenSample.Api.Infrastructure;
using RefreshTokenSample.Api.Models.Commands;
using RefreshTokenSample.Api.Models.Dtos;
using RefreshTokenSample.Api.Models.Entities;
using RefreshTokenSample.Api.Models.Queries;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Windows.Input;

namespace RefreshTokenSample.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IDispatcher _dispatcher;
        private readonly ITokenService _tokenService;

        public AuthController(ILogger<AuthController> logger, IDispatcher dispatcher, ITokenService tokenService)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto loginDto)
        {
            IQueryResult<User> result = _dispatcher.Dispatch(new LoginQuery(loginDto.Email, loginDto.Passwd.Hash()));

            if(result.IsFailure)
            {
                return Unauthorized(new { Error =  result.ErrorMessage });
            }

            _tokenService.GenerateToken(result.Content!);

            return Ok(result.Content);
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto registerDto)
        {
            ICommandResult result = _dispatcher.Dispatch(new RegisterCommand(registerDto.Nom, registerDto.Prenom, registerDto.Email, registerDto.Passwd.Hash()));

            if(result.IsFailure)
            {
                return BadRequest(result);
            }

            return NoContent();
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(TokensDto tokensDto)
        {
            IQueryResult<User> result = _dispatcher.Dispatch(new GetUserByTokensQuery(tokensDto.Token, tokensDto.RefreshToken));

            if (result.IsFailure)
            {
                return Unauthorized("Paire de token invalide");
            }

            User user = result.Content!;
            TokenPair tokenPair = _tokenService.RefreshTokenPair(tokensDto.Token);

            user.Token = tokenPair.Token;
            user.RefreshToken = tokenPair.RefreshToken;

            return Ok(tokenPair);
        }

    }
}
