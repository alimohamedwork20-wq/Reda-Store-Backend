using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationOtp : AbstractValidator<SendOtpDto>
    {
        public ValidationOtp()
        {

            RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Action)
                .NotEmpty().WithMessage("Action is required.")
                .Must(a => a == "twoFactor" || a == "resetPassword" || a == "changeEmail" || a == "changePhone" || a == "addPhone")
                .WithMessage("Invalid OTP action."); ;
        }

    }
}
