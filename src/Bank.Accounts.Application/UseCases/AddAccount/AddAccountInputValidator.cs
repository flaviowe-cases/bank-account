using FluentValidation;

namespace Bank.Accounts.Application.UseCases.AddAccount;

public class AddAccountInputValidator : AbstractValidator<AddAccountInput>
{
    public AddAccountInputValidator()
    {
        RuleFor(p => p.AccountId)
            .NotEmpty()
            .WithErrorCode("ACCOUNT_ID_IS_REQUIRED")
            .WithMessage("Account ID is required");

        RuleFor(p => p.Name)
            .NotEmpty()
            .WithErrorCode("ACCOUNT_NAME_IS_REQUIRED")
            .WithMessage("Account name is required")

            .MinimumLength(3)
            .WithErrorCode("ACCOUNT_NAME_MIN_LENGTH")
            .WithMessage("Account name must be at least 3 characters")

            .MaximumLength(150)
            .WithErrorCode("ACCOUNT_NAME_MAX_LENGTH")
            .WithMessage("Account name must be at most 150 characters");    
        
        RuleFor(p => p.AccountNumber)
            .NotEmpty()
            .WithErrorCode("ACCOUNT_NUMBER_IS_REQUIRED")
            .WithMessage("Account number is required")

            .GreaterThan(0)
            .WithErrorCode("ACCOUNT_NUMBER_IS_GREATER_THAN_ZERO")
            .WithMessage("Account number must be greater than 0");
        
        RuleFor(p => p.Amount)
            .GreaterThan(0)
            .WithErrorCode("ACCOUNT_AMOUNT_IS_INVALID")
            .WithMessage("Account amount is invalid");
    }
}