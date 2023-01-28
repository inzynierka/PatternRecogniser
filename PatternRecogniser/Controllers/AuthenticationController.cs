using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PatternRecogniser.Messages.Authorization;
using PatternRecogniser.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using PatternRecogniser.Services;
using PatternRecogniser.Services.NewFolder;
using PatternRecogniser.Errors;
using PatternRecogniser.Services.Repos;

namespace PatternRecogniser.Controllers
{

    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;
        private readonly IGenericRepository<User> _authenticationRepo;

        public AuthenticationController(IAuthenticationServices authenticationServicis, IGenericRepository<User> authenticationRepo)
        {
            _authenticationServices = authenticationServicis;
            _authenticationRepo = authenticationRepo;
        }


        /// <summary>
        /// Rejestracja 
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUp info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToAdd = _authenticationServices.CreateUserFromSignUpInfo(info);


            Tokens tokens = _authenticationServices.CreateTokens(userToAdd);
            // dodawanie refreshe token do bazy
            _authenticationServices.AddRefreshTokenToUser(tokens.refreshToken, userToAdd);

            _authenticationRepo.Insert(userToAdd);
            await _authenticationRepo.SaveChangesAsync();

            return Ok(tokens);
        }


        /// <summary>
        /// Logowanie
        /// </summary>
        /// <description></description>
        /// <returns></returns>
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LogIn info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _authenticationRepo.Get(u => u.login == info.login).First();

            var tokens = _authenticationServices.CreateTokens(user);

            _authenticationServices.AddRefreshTokenToUser(tokens.refreshToken, user);
            _authenticationServices.RehashUserPassword(info.password, user);

            _authenticationRepo.Update(user);
            await _authenticationRepo.SaveChangesAsync();
            
            return Ok(new LogInRespond()
            {
                tokens = tokens,
                email = user.email
            });
        }




    }
}
