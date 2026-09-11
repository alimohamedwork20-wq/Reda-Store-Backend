using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationUpdateUser : AbstractValidator<UpdateUserDto>
    {
        public ValidationUpdateUser()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .MinimumLength(3)
                .MaximumLength(100)
                .When(x => x.Name != null);

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Role)
                .MaximumLength(30)
                .When(x => x.Role != null);

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .MaximumLength(100)
                .When(x => x.Password != null);
        }
    }
}
