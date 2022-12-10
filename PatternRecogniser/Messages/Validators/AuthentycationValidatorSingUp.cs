using PatternRecogniser.Messages.Authorization;
using FluentValidation;
using System.Linq;

namespace PatternRecogniser.Models.Validators
{
    public class AuthentycationValidatorSingUp : AbstractValidator<SignUp>
    {
        private AuthenticationStringMesseges _message = new AuthenticationStringMesseges();
        public AuthentycationValidatorSingUp(PatternRecogniserDBContext dbContext)
        {
            RuleFor(x => x.email).NotEmpty().EmailAddress();

            RuleFor(x => x.password).MinimumLength(8);

            RuleFor(x => x.email).
                Custom((value, context) =>
            {
                
                bool isEmailTaken = dbContext.user.Where(user => user.email == value).FirstOrDefault() != null;
                if (isEmailTaken)
                    context.AddFailure("email", _message.emailIsTaken);
            });

            RuleFor(x => x.login).
                Custom((value, context) =>
                {
                    bool isLoginTaken = dbContext.user.Where(user => user.login == value).FirstOrDefault() != null;
                    if (isLoginTaken)
                        context.AddFailure("login", _message.loginIsTaken);
                });
        }
    }
}
