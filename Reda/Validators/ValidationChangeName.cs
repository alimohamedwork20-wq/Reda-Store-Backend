using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationChangeName : AbstractValidator<ChangeNameDto>
    {
        public ValidationChangeName()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);
        }
    }
}
