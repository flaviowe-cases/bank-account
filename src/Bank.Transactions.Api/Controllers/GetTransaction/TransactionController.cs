using Asp.Versioning;
using Bank.Commons.Applications.Factories.Results;
using Bank.Transactions.Application.UseCases.GetTransaction;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Transactions.Api.Controllers.GetTransaction;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TransactionController(
    IGetTransactionUseCase getTransactionUseCase) : Controller
{
    private readonly IGetTransactionUseCase _getTransactionUseCase = getTransactionUseCase;

    [HttpGet("{transactionId:guid}")]
    [EndpointDescription("Get transaction by id")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync([FromRoute] Guid transactionId)
    {
        var input = new GetTransactionInput()
            { TransactionId = transactionId };

        var result = await _getTransactionUseCase
            .HandleAsync(input);

        return CreateResponse(result);
    }

    private IActionResult CreateResponse(Result<GetTransactionOutput> result)
    {
        if (result.Success)
            return Ok(result.GetContent());

        if (result.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(result.Failures);

        if (result.ContainsFailure("TRANSACTION_NOT_FOUND"))
            return NotFound(result.Failures);

        return StatusCode(500, result.Failures);
    }
}