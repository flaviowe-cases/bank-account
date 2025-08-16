using Asp.Versioning;
using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.UseCases.ExecuteTransaction;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Transactions.Api.Controllers.ExecuteTransaction;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TransactionController(
    IExecuteTransactionUseCase executeTransactionUseCase) : Controller
{
    private readonly IExecuteTransactionUseCase _executeTransactionUseCase = executeTransactionUseCase;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PostAsync([FromBody] ExecuteTransactionRequest request)
    {
        var input = new ExecuteTransactionInput()
        {
            SourceAccountNumber = request.SourceAccountNumber,
            DestinationAccountNumber = request.DestinationAccountNumber,
            Amount = request.Amount,
            Comments = request.Comments,
        };
        
        var output = await _executeTransactionUseCase
            .HandleAsync(input);
        
        return CreateResponse(output);
    }

    private IActionResult CreateResponse(Result<ExecuteTransactionOutput> output)
    {
        if (output.Success)
            return Created();
        
        if (output.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(output.Failures);

        if (output.ContainsFailure("SOURCE_ACCOUNT_NOT_FOUNT"))
            return NotFound(output.Failures);

        if (output.ContainsFailure("DESTINATION_ACCOUNT_NOT_FOUNT"))
            return NotFound(output.Failures);

        if (output.ContainsFailure("INSUFFICIENT_FUNDS"))
            return StatusCode(422, output.Failures);

        if (output.ContainsFailure("LIMIT_EXCEEDED"))
            return StatusCode(422, output.Failures);

        if (output.ContainsFailure("TRANSFER_FAILED"))
            return StatusCode(422, output.Failures);

        if (output.ContainsFailure("SERVICE_TEMPORARILY_UNAVAILABLE"))
            return StatusCode(503, output.Failures);
        
        return StatusCode(500, output.Failures); 
    }
}