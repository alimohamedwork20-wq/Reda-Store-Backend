using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationRegister : AbstractValidator<RegisterDto>
    {
        public ValidationRegister()
        {
            RuleFor(x=> x.Email).NotEmpty().EmailAddress();
            RuleFor(x=> x.Password).NotEmpty();
            RuleFor(x=> x.Name).NotEmpty().MinimumLength(3);
            

        }
    }
}
