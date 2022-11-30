using Microsoft.AspNetCore.Mvc;
using PatternRecogniser.Messages.Authorization;
using PatternRecogniser.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PatternRecogniser.Controllers
{

    public class AuthenticationController : ControllerBase
    {
        private PatternRecogniserDBContext _context;
        private AuthenticationStringMesseges _message = new AuthenticationStringMesseges();
        public AuthenticationController(PatternRecogniserDBContext context)
        {
            _context = context;
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
                bool isLoginTaken = _context.user.Where(user => user.login == info.login).FirstOrDefault() != null;
                bool isEmailTaken = _context.user.Where(user => user.email == info.email).FirstOrDefault() != null;
                if (isLoginTaken)
                    return BadRequest(_message.loginIsTaken);
                

                if (isEmailTaken)
                    return BadRequest(_message.emailIsTaken);


                _context.user.Add(new User()
                {
                    createDate = DateTime.Now,
                    lastLog = DateTime.Now,
                    login = info.login,
                    email = info.email
                });


                string seed = CreateSeed();
                var authentication = new Authentication()
                {
                    userLogin = info.login,
                    lastSeed = seed, // tutaj jakaś funkcja losowa 
                    hashedToken = CreatedToken(info.password + seed)
                };
                _context.authentication.Add(authentication);

                await _context.SaveChangesAsync();
                return Ok(new Token()
                {
                    accessToken = authentication.hashedToken
                });
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
                    NotFound(_message.userNotFound);


                var authorization = _context.authentication.Where(authentication => authentication.userLogin == info.login).First();

                if (!CheckIfPasswordIsCorrect(info.password, authorization))
                    return BadRequest(_message.incorectPassword);


                string seed = CreateSeed();
                authorization.lastSeed = seed;
                authorization.hashedToken = CreatedToken(info.password + seed);

                await _context.SaveChangesAsync();
                return Ok(new Token()
                {
                    accessToken = authorization.hashedToken
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private string CreatedToken(string password)
        {
            return password; // tutaj będziemy kodować hasło
        }

        private string CreateSeed()
        {
            return "ziarno";
        }

        public bool CheckIfPasswordIsCorrect(string password, Authentication authentication)
        {
            return authentication.hashedToken == CreatedToken(password + authentication.lastSeed); // tutaj będziemy kodować hasło
        }

    }
}
