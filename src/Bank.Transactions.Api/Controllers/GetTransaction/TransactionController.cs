using Asp.Versioning;
using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.UseCases.GetTransaction;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Transactions.Api.Controllers.GetTransaction;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TransactionController
    (IGetTransactionUseCase getTransactionUseCase): Controller
{
    private readonly IGetTransactionUseCase _getTransactionUseCase = getTransactionUseCase;

    [HttpGet("{transactionId:guid}")]
    [EndpointDescription("Get a transaction")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(Guid transactionId)
    {
        var output = await _getTransactionUseCase.HandleAsync(new()
        {
            TransactionId = transactionId
        });
        
        return CreateResponse(output);
    }

    private IActionResult CreateResponse(Result<GetTransactionOutput> output)
    {
        if (output.Success)
            return Ok(output.GetContent());
        
        if (output.ContainsFailure("TRANSACTION_NOT_FOUND"))
            return NotFound(output.Failures);
        
        return StatusCode(500, output.Failures); 
    }
}