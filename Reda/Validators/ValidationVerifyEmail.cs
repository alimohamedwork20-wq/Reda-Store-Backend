using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationVerifyEmail : AbstractValidator<VerifyEmailDto>
    {
        public ValidationVerifyEmail()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);

            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Code)
                .NotEmpty()
                .Matches(@"^[0-9]{6}$")
                .WithMessage("OTP code must be exactly 6 digits.");
        }
    }
}
