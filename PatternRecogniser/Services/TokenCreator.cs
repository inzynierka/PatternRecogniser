using Microsoft.IdentityModel.Tokens;
using PatternRecogniser.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PatternRecogniser.Services
{
    public interface ITokenCreator
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }

    public class TokenCreator : ITokenCreator
    {
        private AuthenticationSettings _authenticationSettings;

        public TokenCreator(AuthenticationSettings authenticationSettings)
        {
            _authenticationSettings = authenticationSettings;
        }

        public string CreateAccessToken(User user)
        {
            var claims = new List<Claim>()
                {
                    new Claim (ClaimTypes.NameIdentifier, user.login)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(_authenticationSettings.JwtExpireTimeInMinutes);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public string CreateRefreshToken(User user)
        {
            return "NotImplementedYet";
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            throw new System.NotImplementedException();
        }
    }
}
