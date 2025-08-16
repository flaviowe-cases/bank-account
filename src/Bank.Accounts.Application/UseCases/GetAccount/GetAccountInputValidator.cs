using FluentValidation;

namespace Bank.Accounts.Application.UseCases.GetAccount;

public class GetAccountInputValidator : AbstractValidator<GetAccountInput>
{
    public GetAccountInputValidator()
    {
        RuleFor(p => p.AccountId)
            .NotEmpty()
            .WithErrorCode("ACCOUNT_ID_IS_REQUIRED")
            .WithMessage("Account ID is required");
    }
}