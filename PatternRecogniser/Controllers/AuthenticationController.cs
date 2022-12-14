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
        private readonly IAuthenticationServicis _authenticationServicis;

        public AuthenticationController(IAuthenticationServicis authenticationServicis)
        {
            _authenticationServicis = authenticationServicis;
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

            var userToAdd = _authenticationServicis.CreateUserFromSignUpInfo(info);


            Tokens tokens = _authenticationServicis.CreateTokens(userToAdd);
            // dodawanie refreshe token do bazy
            _authenticationServicis.AddRefreshTokenToUser(tokens.refreshToken, userToAdd);

            await _authenticationServicis.SaveUser(userToAdd);


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
           // return NotFound("asdasd");
            var respond = await _authenticationServicis.LogIn(info);
            
            return Ok(respond);
        }




    }
}
