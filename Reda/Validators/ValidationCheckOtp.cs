using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationCheckOtp : AbstractValidator<CheckOtpDto>
    {
        public ValidationCheckOtp()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Code)
                .NotEmpty()
                .Matches(@"^[0-9]{6}$")
                .WithMessage("OTP code must be exactly 6 digits.");

            RuleFor(x => x.Action)
                .NotEmpty()
                .Must(a => a == "twoFactor" || a == "resetPassword" || a == "changeEmail" || a == "changePhone" || a == "addPhone")
                .WithMessage("Invalid OTP action.");
        }
    }
}
