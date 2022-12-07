using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PatternRecogniser.Messages.Authorization;
using PatternRecogniser.Models;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using PatternRecogniser.Services;

namespace PatternRecogniser.Controllers
{

    public class AuthenticationController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        private AuthenticationStringMesseges _message = new AuthenticationStringMesseges();
        private IPasswordHasher<User> _passwordHasher;
        private ITokenCreator _tokenCreator;

        public AuthenticationController(PatternRecogniserDBContext context, IPasswordHasher<User> passwordHasher, ITokenCreator tokenCreator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenCreator = tokenCreator;
        }


        /// <summary>
        /// Rejestracja 
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUp info)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

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
                var refreshToken = _tokenCreator.CreateRefreshToken(userToAdd);
                // dodawanie refreshe token do bazy

                await _context.SaveChangesAsync();
                 return Ok(new AuthenticationRespond()
                {
                    accessToken = accesToken,
                    refreshToken = refreshToken
                 });;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// Logowanie
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogIn info)
        {
            try
            {

                var user = _context.user.Where(user => user.login == info.login).FirstOrDefault();
                if (user == null)
                    return NotFound(_message.userNotFound);



                if (_passwordHasher.VerifyHashedPassword(user, user.hashedPassword, info.password) == PasswordVerificationResult.Failed)
                    return BadRequest(_message.incorectPassword);



                var accesToken = _tokenCreator.CreateAccessToken(user);
                var refreshToken = _tokenCreator.CreateRefreshToken(user);
                // dodawanie refreshe token do bazy

                return Ok(new AuthenticationRespond()
                {
                    accessToken = accesToken,
                    refreshToken = refreshToken
                });;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }




    }
}
