using FluentValidation;
using Reda.Dtos;

namespace Reda.Validators
{
    public class ValidationReport : AbstractValidator<ReportDto>
    {
        public ValidationReport()
        {
            RuleFor(x => x.Category)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Subject)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MinimumLength(10)
                .MaximumLength(2000);

            RuleFor(x => x.Screenshot)
                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                .WithMessage("Screenshot size must not exceed 5 MB.");
        }
    }
}
