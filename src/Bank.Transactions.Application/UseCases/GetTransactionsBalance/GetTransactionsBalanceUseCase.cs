using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.Services;

namespace Bank.Transactions.Application.UseCases.GetTransactionsBalance;

public class GetTransactionsBalanceUseCase(
    IAmountService  amountService,
    IResultFactory resultFactory) : IGetTransactionsBalanceUseCase
{
    private readonly IAmountService _amountService = amountService;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<GetTransactionsBalanceOutput>> HandleAsync(
        GetTransactionsBalanceInput input)
    {
       var tasks = input.AccountsId
           .Select(accountId => _amountService.GetCurrentBalanceAsync(accountId)).ToList();

       var accountBalance = await Task.WhenAll(tasks);

        return _resultFactory.CreateSuccess(
            new GetTransactionsBalanceOutput()
            {
                AccountBalance = accountBalance
                    .ToList()
            });
    }
}