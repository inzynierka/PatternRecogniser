using Microsoft.AspNetCore.Identity;
using PatternRecogniser.Errors;
using PatternRecogniser.Messages.Authorization;
using PatternRecogniser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Services
{
    public interface IAuthenticationServicis
    {


        void AddRefreshTokenToUser(string refreshToken, User userToAdd);
        Tokens CreateTokens(User userToAdd);
        User CreateUserFromSignUpInfo(SignUp info);
    }

    public class AuthenticationServicis: IAuthenticationServicis
    {
        private IPasswordHasher<User> _passwordHasher;
        private ITokenCreator _tokenCreator;

        public AuthenticationServicis(IPasswordHasher<User> passwordHasher, ITokenCreator tokenCreator)
        {
            _passwordHasher = passwordHasher;
            _tokenCreator = tokenCreator;
        }

        public User CreateUserFromSignUpInfo(SignUp info)
        {
            var user = new User()
            {
                createDate = DateTime.Now,
                lastLog = DateTime.Now,
                login = info.login,
                email = info.email
            };

            // samo dodaje ziarno więc luzik
            user.hashedPassword = _passwordHasher.HashPassword(user, info.password);

            return user;
        }


        public Tokens CreateTokens(User user)
        {
            var accesToken = _tokenCreator.CreateAccessToken(user);
            var refreshToken = _tokenCreator.CreateRefreshToken();
            return new Tokens()
            {
                accessToken = accesToken,
                refreshToken = refreshToken
            }; ;
        }

        public void AddRefreshTokenToUser(string refreshToken, User user)
        {
            user.refreshToken = _passwordHasher.HashPassword(user, refreshToken);
            user.refreshTokenExpiryDate = _tokenCreator.RefresheTokenExpireDate();
        }



       
    }
}
