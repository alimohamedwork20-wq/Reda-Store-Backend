using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationSendOtpToEmail : AbstractValidator<SendOtpTOEmailDto>
    {
        public ValidationSendOtpToEmail()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
