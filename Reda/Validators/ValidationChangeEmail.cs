using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationChangeEmail : AbstractValidator<ChangeEmailDto>
    {
        public ValidationChangeEmail()
        {
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
