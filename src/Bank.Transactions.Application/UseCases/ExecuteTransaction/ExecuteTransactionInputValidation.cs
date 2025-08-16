using FluentValidation;

namespace Bank.Transactions.Application.UseCases.ExecuteTransaction;

public class ExecuteTransactionInputValidation 
    : AbstractValidator<ExecuteTransactionInput>
{
    public ExecuteTransactionInputValidation()
    {
        RuleFor(x => x)
            .Must(x =>
                x.SourceAccountNumber is not null ||
                x.DestinationAccountNumber is not null)
            .WithErrorCode("SOURCE_ACCOUNT_OR_DESTINATION_ACCOUNT_REQUIRED")
            .WithMessage("SourceAccountNumber or DestinationAccountNumber cannot be null or empty.");

        RuleFor(x => x.SourceAccountNumber)
            .GreaterThan(0)
            .When(x => x.SourceAccountNumber is not null)
            .WithErrorCode("SOURCE_ACCOUNT_MUST_GREATER_THAN_0")
            .WithMessage("Source account number must be greater than 0.");

        RuleFor(x => x.DestinationAccountNumber)
            .GreaterThan(0)
            .When(x => x.DestinationAccountNumber is not null)
            .WithErrorCode("DESTINATION_ACCOUNT_MUST_GREATER_THAN_0")
            .WithMessage("Destination account number must be greater than 0.");
        
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithErrorCode("AMOUNT_ACCOUNT_MUST_GREATER_THAN_0")
            .WithMessage("Amount account number must be greater than 0.");
    }
    
}