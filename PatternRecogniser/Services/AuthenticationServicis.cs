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
        public  Task<Tokens> SignUp(SignUp info);

        public Task<LogInRespond> LogIn(LogIn info);

    }

    public class AuthenticationServicis: IAuthenticationServicis
    {
        private PatternRecogniserDBContext _context;
        private AuthenticationStringMesseges _message = new AuthenticationStringMesseges();
        private IPasswordHasher<User> _passwordHasher;
        private ITokenCreator _tokenCreator;

        public AuthenticationServicis(PatternRecogniserDBContext context, IPasswordHasher<User> passwordHasher, ITokenCreator tokenCreator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenCreator = tokenCreator;
        }


        public async Task<Tokens> SignUp( SignUp info)
        {
                

                var userToAdd = new User()
                {
                    createDate = DateTime.Now,
                    lastLog = DateTime.Now,
                    login = info.login,
                    email = info.email
                };

                // samo dodaje ziarno więc luzik
                userToAdd.hashedPassword = _passwordHasher.HashPassword(userToAdd, info.password);
                _context.user.Add(userToAdd);

                var accesToken = _tokenCreator.CreateAccessToken(userToAdd);
                var refreshToken = _tokenCreator.CreateRefreshToken();
                // dodawanie refreshe token do bazy
                userToAdd.refreshToken = _passwordHasher.HashPassword(userToAdd, refreshToken);
                userToAdd.refreshTokenExpiryDate = _tokenCreator.RefresheTokenExpireDate();

                await _context.SaveChangesAsync();
                return new Tokens()
                {
                    accessToken = accesToken,
                    refreshToken = refreshToken
                }; ;
        }


        public async Task<LogInRespond> LogIn( LogIn info)
        {
                var user = _context.user.Where(user => user.login == info.login).FirstOrDefault();
                if (user == null)
                    throw new NotFoundExeption (_message.userNotFound);



                if (_passwordHasher.VerifyHashedPassword(user, user.hashedPassword, info.password) == PasswordVerificationResult.Failed)
                    throw new BadRequestExeption(_message.incorectPassword);



                var accesToken = _tokenCreator.CreateAccessToken(user);
                var refreshToken = _tokenCreator.CreateRefreshToken();


                user.refreshToken = _passwordHasher.HashPassword(user, refreshToken);
                user.refreshTokenExpiryDate = _tokenCreator.RefresheTokenExpireDate();
                await _context.SaveChangesAsync();

                return new LogInRespond()
                {
                    tokens = new Tokens()
                    {
                        accessToken = accesToken,
                        refreshToken = refreshToken
                    },
                    email = user.email
                }; 
        }
    }
}
