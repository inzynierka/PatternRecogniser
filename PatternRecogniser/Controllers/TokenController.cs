using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Messages.Authorization;
using PatternRecogniser.Messages.Token;
using PatternRecogniser.Models;
using PatternRecogniser.Services;
using PatternRecogniser.Services.Repos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{

   [ Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly ITokenCreator _tokenCreator;
        private IPasswordHasher<User> _passwordHasher;
        private readonly TokenStringMesseges _messeges = new TokenStringMesseges();

        public TokenController(IGenericRepository<User> userRepo, ITokenCreator tokenCreator, IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _tokenCreator = tokenCreator;
            _passwordHasher = passwordHasher;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] Tokens tokens)
        {
            if (tokens is null)
                return Unauthorized(_messeges.invalidClientRequest);
            string accessToken = tokens.accessToken;
            string refreshToken = tokens.refreshToken;

            var principal = _tokenCreator.GetPrincipalFromExpiredToken(accessToken);
            var login = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = _userRepo.Get(u => u.login == login).SingleOrDefault();

            if (user is null ||
                _passwordHasher.VerifyHashedPassword(user, user.refreshToken, refreshToken) == PasswordVerificationResult.Failed||
                user.refreshTokenExpiryDate <= DateTime.Now)
                return Unauthorized(_messeges.invalidClientRequest);

            var newAccessToken = _tokenCreator.CreateAccessToken(user);
            var newRefreshToken = _tokenCreator.CreateRefreshToken();
            user.refreshToken = _passwordHasher.HashPassword(user, newRefreshToken);
            user.refreshTokenExpiryDate = _tokenCreator.RefresheTokenExpireDate();
            await _userRepo.SaveChangesAsync();

            return Ok(new Tokens()
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }


        [HttpPost,
        Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var login = User.Identity.Name;
            var user = _userRepo.Get(u => u.login == login).SingleOrDefault();
            if (user == null) return BadRequest();
            user.refreshToken = null;
            await _userRepo.SaveChangesAsync();
            return NoContent();
        }

    }
}
