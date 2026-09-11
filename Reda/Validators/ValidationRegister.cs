using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationRegister : AbstractValidator<RegisterDto>
    {
        public ValidationRegister()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);
        }
    }
}
