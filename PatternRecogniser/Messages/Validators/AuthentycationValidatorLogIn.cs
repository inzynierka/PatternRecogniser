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

            RuleFor(x => x.login).
               Custom((value, context) =>
               {
                   user = AuthenticationRepo.GetUsers(u => u.login == value).FirstOrDefault();
                   if (user != null)
                       context.AddFailure("login", _message.userNotFound);
               });

            
                RuleFor(x => x.password).
                    Custom((value, context) =>
                    {
                        user = AuthenticationRepo.GetUsers(u => u.login == value).FirstOrDefault();
                        if (user != null)
                            if (passwordHasher.VerifyHashedPassword(user, user.hashedPassword, value) == PasswordVerificationResult.Failed)
                                context.AddFailure("password", _message.incorectPassword);
                    });
        }
    }
}
