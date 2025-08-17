using Asp.Versioning;
using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.UseCases.GetTransactionsBalance;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Transactions.Api.Controllers.GetTransactionBalance;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TransactionController(
    IGetTransactionsBalanceUseCase getTransactionsBalanceUseCase) : ControllerBase
{
    private readonly IGetTransactionsBalanceUseCase _getTransactionsBalanceUseCase = getTransactionsBalanceUseCase;

    [HttpGet("Balance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTransactionsBalanceOutput))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultFail[]))]
    public async Task<IActionResult> GetAsync([FromQuery] Guid[] accountId)
    {
        var output = await _getTransactionsBalanceUseCase
            .HandleAsync(new GetTransactionsBalanceInput
            {
                AccountsId = accountId
            });

        return CreateResponse(output);
    }

    private IActionResult CreateResponse(Result<GetTransactionsBalanceOutput> output)
    {
        if (output.Success)
            return Ok(output.GetContent());

        return StatusCode(500, output.Failures);
    }
}