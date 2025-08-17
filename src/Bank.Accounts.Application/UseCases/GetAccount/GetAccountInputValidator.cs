using FluentValidation;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountInputValidator : AbstractValidator<GetAccountInput>
{
    public GetAccountInputValidator()
    {
        RuleFor(p => p)
            .Must(p => p.AccountId.HasValue || p.AccountNumber.HasValue)
            .WithErrorCode("ACCOUNT_ID_OR_ACCOUNT_NUMBER_REQUIRED")
            .WithMessage("Account id or account number required");
        
        RuleFor(p => p.AccountId)
            .NotEmpty()
            .When(x => x.AccountId.HasValue)
            .WithErrorCode("ACCOUNT_ID_IS_REQUIRED")
            .WithMessage("Account ID is required");
        
        RuleFor(p => p.AccountNumber)
            .GreaterThan(0)
            .When(x => x.AccountNumber.HasValue)
            .WithErrorCode("ACCOUNT_ID_IS_REQUIRED")
            .WithMessage("Account ID is required");

    }
}