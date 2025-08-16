using FluentValidation;

namespace Bank.Accounts.Application.UseCases.ListAccounts;

public class ListAccountInputValidator : AbstractValidator<ListAccountsInput>
{
    public ListAccountInputValidator()
    {
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