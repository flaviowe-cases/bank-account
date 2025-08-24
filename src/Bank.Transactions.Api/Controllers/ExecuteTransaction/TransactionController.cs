using Asp.Versioning;
using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.UseCases.CreateTransaction;
using Bank.Transactions.Application.UseCases.ExecuteTransaction;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Transactions.Api.Controllers.ExecuteTransaction;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TransactionController(
    ICreateTransactionUseCase createTransactionUseCase) : Controller
{
    private readonly ICreateTransactionUseCase _createTransactionUseCase = createTransactionUseCase;

    [HttpPost]
    [EndpointDescription("Creates a new transaction")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostAsync([FromBody] ExecuteTransactionRequest request)
    {
        var input = new CreateTransactionInput()
        {
            SourceAccountNumber = request.SourceAccountNumber,
            DestinationAccountNumber = request.DestinationAccountNumber,
            Amount = request.Amount,
            Comments = request.Comments,
        };
        
        var output = await _createTransactionUseCase
            .HandleAsync(input);
        
        return CreateResponse(output);
    }

    private IActionResult CreateResponse(Result<CreateTransactionOutput> output)
    {
        if (output.Success)
            return Created();
        
        if (output.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(output.Failures);

        if (output.ContainsFailure("SOURCE_ACCOUNT_NOT_FOUNT"))
            return NotFound(output.Failures);

        if (output.ContainsFailure("DESTINATION_ACCOUNT_NOT_FOUNT"))
            return NotFound(output.Failures);
        
        if (output.ContainsFailure("SERVICE_TEMPORARILY_UNAVAILABLE"))
            return StatusCode(503, output.Failures);
        
        return StatusCode(500, output.Failures); 
    }
}