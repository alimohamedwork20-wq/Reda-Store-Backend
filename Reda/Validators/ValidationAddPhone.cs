using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationAddPhone : AbstractValidator<AddPhoneDto>
    {
        public ValidationAddPhone()
        {
            RuleFor(x => x.Phone)
                .NotEmpty()
                .Matches(@"^\+?[0-9]{10,15}$")
                .WithMessage("Phone must contain 10 to 15 digits.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Code)
                .NotEmpty()
                .Matches(@"^[0-9]{6}$")
                .WithMessage("OTP code must be exactly 6 digits.");
        }
    }
}
