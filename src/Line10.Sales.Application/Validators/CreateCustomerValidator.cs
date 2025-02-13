using FluentValidation;
using Line10.Sales.Application.Commands;

namespace Line10.Sales.Application.Validators;

public class CreateCustomerValidator: AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithErrorCode("InvalidFirstName")
            .WithMessage("FirstName cannot be empty");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithErrorCode("InvalidLastName")
            .WithMessage("LastName cannot be empty");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode("InvalidEmail")
            .WithMessage("Email cannot be empty");
    }
}
