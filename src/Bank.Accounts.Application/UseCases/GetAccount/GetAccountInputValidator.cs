using FluentValidation;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountInputValidator : AbstractValidator<GetAccountInput>
{
    public GetAccountInputValidator()
    {
        RuleFor(p => p.AccountNumber)
            .NotEmpty()
            .WithErrorCode("ACCOUNT_NUMBER_IS_REQUIRED")
            .WithMessage("Account number is required")

            .GreaterThan(0)
            .WithErrorCode("ACCOUNT_NUMBER_IS_GREATER_THAN_ZERO")
            .WithMessage("Account number must be greater than 0");
    }
    
}