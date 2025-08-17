using FluentValidation;

namespace Bank.Transactions.Application.UseCases.GetTransactionsHistory;

public class GetTransactionsHistoryInputValidator : AbstractValidator<GetTransactionsHistoryInput>
{
    public GetTransactionsHistoryInputValidator()
    {
        RuleFor(x => x.AccountNumber)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ACCOUNT_NUMBER")
            .WithMessage("Account number must be greater than 0");
    }
}