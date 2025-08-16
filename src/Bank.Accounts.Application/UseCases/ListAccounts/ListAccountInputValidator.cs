using FluentValidation;

namespace Bank.Accounts.Application.UseCases.ListAccounts;

public class ListAccountInputValidator : AbstractValidator<ListAccountsInput>
{
    public ListAccountInputValidator()
    {
        RuleFor(p => p.AccountNumber)
            .GreaterThan(0)
            .When(p => p.AccountNumber != null)
            .WithErrorCode("ACCOUNT_NUMBER_IS_GREATER_THAN_ZERO")
            .WithMessage("Account number must be greater than 0");
        
        RuleFor(p => p.PageNumber)
            .GreaterThan(0)
            .WithErrorCode("PAGE_NUMBER_GREATER_THAN_ZERO")
            .WithMessage("Page number must be greater than zero");
        
        RuleFor(p => p.PageSize)
            .GreaterThan(0)
            .WithErrorCode("PAGE_SIZE_GREATER_THAN_ZERO")
            .WithMessage("Page size must be greater than zero");
    }
}