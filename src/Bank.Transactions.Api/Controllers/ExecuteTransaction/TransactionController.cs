using Asp.Versioning;
using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.UseCases.CreateTransaction;
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
        
        var result = await _createTransactionUseCase
            .HandleAsync(input);
        
        return CreateResponse(result);
    }

    private IActionResult CreateResponse(Result<CreateTransactionOutput> result)
    {
        if (result.Success)
        {
            var output = result.GetContent();
            var uri = new Uri($"Transaction/{output.TransactionId}");
            return Created(uri, output);
        }
        
        if (result.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(result.Failures);

        if (result.ContainsFailure("SOURCE_ACCOUNT_NOT_FOUNT"))
            return NotFound(result.Failures);

        if (result.ContainsFailure("DESTINATION_ACCOUNT_NOT_FOUNT"))
            return NotFound(result.Failures);
        
        if (result.ContainsFailure("SERVICE_TEMPORARILY_UNAVAILABLE"))
            return StatusCode(503, result.Failures);
        
        return StatusCode(500, result.Failures); 
    }
}