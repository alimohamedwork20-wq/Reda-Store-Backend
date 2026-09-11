using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationResetPassword : AbstractValidator<ResetPasswordDto>
    {
        public ValidationResetPassword()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}
