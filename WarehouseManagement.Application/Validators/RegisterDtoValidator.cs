using FluentValidation;
using WarehouseManagement.Application.DTOs.Users;

namespace WarehouseManagement.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .Must(email =>
                      email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase) ||
                      email.EndsWith("@outlook.com", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only Gmail or Outlook emails are allowed.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}
