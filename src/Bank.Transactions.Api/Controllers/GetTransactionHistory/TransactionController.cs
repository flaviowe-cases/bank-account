using Asp.Versioning;
using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.UseCases.GetTransactionsHistory;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Transactions.Api.Controllers.GetTransactionHistory;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TransactionController(
    IGetTransactionsHistoryUseCase getTransactionsHistoryUseCase) : Controller
{
    private readonly IGetTransactionsHistoryUseCase _getTransactionsHistoryUseCase = getTransactionsHistoryUseCase;

    [HttpGet("History/{accountNumber:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTransactionsHistoryOutput))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultFail[]))]
    public async Task<IActionResult> GetAsync(int accountNumber)
    {
        var input = new GetTransactionsHistoryInput()
        {
            AccountNumber = accountNumber
        };

        var output = await _getTransactionsHistoryUseCase
            .HandleAsync(input);
        
        return CreateResponse(output);
    }

    private IActionResult CreateResponse(Result<GetTransactionsHistoryOutput> output)
    {
        if (output.Success)
            return Ok(output.GetContent());
        
        if (output.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(output.Failures); 
        
        if (output.ContainsFailure("ACCOUNT_NOT_FOUND"))
            return NotFound(output.Failures); 
        
        if (output.ContainsFailure("SERVICE_TEMPORARILY_UNAVAILABLE"))
            return StatusCode(503, output.Failures); 
        
        return StatusCode(500, output.Failures);
    }
}