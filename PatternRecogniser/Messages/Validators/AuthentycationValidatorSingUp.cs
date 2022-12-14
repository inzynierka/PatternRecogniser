using PatternRecogniser.Messages.Authorization;
using FluentValidation;
using System.Linq;
using PatternRecogniser.Services;

namespace PatternRecogniser.Models.Validators
{
    public class AuthentycationValidatorSingUp : AbstractValidator<SignUp>
    {
        private AuthenticationStringMesseges _message = new AuthenticationStringMesseges();
        public AuthentycationValidatorSingUp(IAuthenticationServicis AuthenticationRepo)
        {
            RuleFor(x => x.email).NotEmpty().EmailAddress();

            RuleFor(x => x.password).MinimumLength(8);

            RuleFor(x => x.email).
                Custom((value, context) =>
            {
                if (AuthenticationRepo.IsEmailTaken(value))
                    context.AddFailure("email", _message.emailIsTaken);
            });

            RuleFor(x => x.login).
                Custom((value, context) =>
                {
                    if  (AuthenticationRepo.IsLoginTaken(value))
                        context.AddFailure("login", _message.loginIsTaken);
                });
        }
    }
}
