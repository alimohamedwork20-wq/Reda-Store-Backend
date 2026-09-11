using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationChangePassword : AbstractValidator<ChangePasswordDto>
    {
        public ValidationChangePassword()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);

            RuleFor(x => x.NewPassword)
                .NotEqual(x => x.OldPassword)
                .WithMessage("New password must be different from old password.");
        }
    }
}
