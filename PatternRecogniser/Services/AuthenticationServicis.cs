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

        public Task<LogInRespond> LogIn(LogIn info);

        public bool IsEmailTaken(string email);
        public bool IsLoginTaken(string login);
        Task SaveUser(User userToAdd);
        void AddRefreshTokenToUser(string refreshToken, User userToAdd);
        Tokens CreateTokens(User userToAdd);
        User CreateUserFromSignUpInfo(SignUp info);
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

        public bool IsEmailTaken(string email)
        {
            return _context.user.Where(user => user.email == email).FirstOrDefault() != null;
        }

        public bool IsLoginTaken(string login)
        {
            return _context.user.Where(user => user.login == login).FirstOrDefault() != null;
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


        public async Task SaveUser(User user)
        {
            _context.user.Add(user);
            await _context.SaveChangesAsync();
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
