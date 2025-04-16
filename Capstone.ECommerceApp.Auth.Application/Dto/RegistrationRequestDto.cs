using FluentValidation;

namespace Capstone.ECommerceApp.Auth.Application.Dto;

public class RegistrationRequestDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string? Role { get; set; }
}

public class RegistrationRequestValidator : AbstractValidator<RegistrationRequestDto>
{
    public RegistrationRequestValidator()
    {
        RuleFor(user => user.Name)
          .NotEmpty()
          .WithMessage("Name is required.");
        RuleFor(user => user.PhoneNumber)
          .NotEmpty()
          .WithMessage("Phone Number is required.");
        RuleFor(user => user.Email)
          .EmailAddress()
          .WithMessage("Invalid email address.");
    }
}
