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
        public AuthenticationController(PatternRecogniserDBContext context)
        {
            _context = context;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUp info)
        {
            try
            {
                _context.user.Add(new User()
                {
                    createDate = DateTime.Now,
                    lastLog = DateTime.Now,
                    login = info.login,
                    email = info.email
                });


                var authentication = new Authentication()
                {
                    userLogin = info.login,
                    lastSeed = "ziarno", // tutaj jakaś funkcja losowa 
                    hashedToken = CreatedToken(info.password + "ziarno")
                };
                _context.authentication.Add(authentication);

                await _context.SaveChangesAsync();
                return Ok(new Respond()
                {
                    accessToken = authentication.hashedToken
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> LogIn([FromBody] LogIn info)
        {
            try
            {

                string login = _context.user.Where(user => user.login == info.login).FirstOrDefault()?.login;
                if (login == null)
                    NotFound("użytkownik nie istnieje");


                var authorization = _context.authentication.Where(authentication => authentication.userLogin == info.login).First();

                if (!CheckIfPasswordIsCorrect(info.password, authorization))
                    return BadRequest("Niepoprawne hasło");

                var newAuthenticationData = new Authentication()
                {
                    userLogin = login,
                    lastSeed = "ziarno", // tutaj jakaś funkcja losowa 
                    hashedToken = CreatedToken(info.password + "ziarno")
                };

                _context.authentication.Remove(authorization);
                _context.authentication.Add(newAuthenticationData);

                await _context.SaveChangesAsync();
                return Ok(new Respond()
                {
                    accessToken = newAuthenticationData.hashedToken
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

        public bool CheckIfPasswordIsCorrect(string password, Authentication authentication)
        {
            return authentication.hashedToken == CreatedToken(password + authentication.lastSeed); // tutaj będziemy kodować hasło
        }

    }
}
