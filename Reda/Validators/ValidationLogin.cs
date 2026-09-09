using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationLogin : AbstractValidator<LoginDto>
    {
        public ValidationLogin()
        {
            RuleFor(u => u.EmailOrPhone).NotEmpty();
            RuleFor(u => u.Password).NotEmpty().MinimumLength(6);
        }

    }
}
