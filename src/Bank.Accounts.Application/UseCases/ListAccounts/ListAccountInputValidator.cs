using FluentValidation;

namespace Bank.Accounts.Application.UseCases.ListAccounts;

public class ListAccountInputValidator : AbstractValidator<ListAccountsInput>
{
    public ListAccountInputValidator()
    {
        RuleFor(p => p.PageNumber)
            .InclusiveBetween(1, 1000)
            .WithErrorCode("PAGE_NUMBER_MUST_BETWEEN_ONE_THOUSAND")
            .WithMessage("Page number must be between 1 and 1000.");

        RuleFor(p => p.PageSize)
            .InclusiveBetween(1, 1000)
            .WithErrorCode("PAGE_SIZE_MUST_BETWEEN_ONE_THOUSAND")
            .WithMessage("Page size must be between 1 and 1000.");
    }
}