﻿using PatternRecogniser.Messages.Authorization;
using FluentValidation;
using System.Linq;
using PatternRecogniser.Services;
using PatternRecogniser.Services.NewFolder;

namespace PatternRecogniser.Models.Validators
{
    public class AuthentycationValidatorSingUp : AbstractValidator<SignUp>
    {
        private AuthenticationStringMesseges _message = new AuthenticationStringMesseges();
        public AuthentycationValidatorSingUp(IAuthenticationRepo AuthenticationRepo)
        {
            RuleFor(x => x.email).NotEmpty().EmailAddress();

            RuleFor(x => x.password).MinimumLength(8);

            RuleFor(x => x.email).
                Custom((value, context) =>
            {
                if (AuthenticationRepo.GetUsers( u => u.email == value).Count > 0)
                    context.AddFailure("email", _message.emailIsTaken);
            });

            RuleFor(x => x.login).
                Custom((value, context) =>
                {
                    if  (AuthenticationRepo.GetUsers(u => u.login == value).Count > 0)
                        context.AddFailure("login", _message.loginIsTaken);
                });
        }
    }


    
}
