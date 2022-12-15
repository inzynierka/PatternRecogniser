using FluentValidation;
using Microsoft.AspNetCore.Identity;
using PatternRecogniser.Messages.Authorization;
using PatternRecogniser.Models;
using PatternRecogniser.Services.NewFolder;
using System.Linq;

namespace PatternRecogniser.Messages.Validators
{
    public class AuthentycationValidatorLogIn : AbstractValidator<LogIn>
    {
        private AuthenticationStringMesseges _message = new AuthenticationStringMesseges();
        public AuthentycationValidatorLogIn(IAuthenticationRepo AuthenticationRepo, IPasswordHasher<User> passwordHasher)
        {

            
            User user = null;
            When(x => {
                user = AuthenticationRepo.GetUsers(u => u.login == x.login).FirstOrDefault();
                return user == null;
            }, ()=>
            {
                RuleFor(l => l.login).Custom((value, context) =>
                {
                    context.AddFailure("login", _message.userNotFound);
                });
            }).Otherwise( () => {
                RuleFor(x => x.password).
                   Custom((value, context) =>
                   {
                       if (passwordHasher.VerifyHashedPassword(user, user.hashedPassword, value) == PasswordVerificationResult.Failed)
                               context.AddFailure("password", _message.incorectPassword);
                   });
            });
            
        }
    }
}
