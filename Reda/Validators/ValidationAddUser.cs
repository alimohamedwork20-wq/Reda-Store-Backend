using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationAddUser : AbstractValidator<AddUserDto>
    {
        public ValidationAddUser()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Role)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(100);
        }
    }
}
