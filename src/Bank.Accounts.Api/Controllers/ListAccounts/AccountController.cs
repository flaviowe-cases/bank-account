using Asp.Versioning;
using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.UseCases.ListAccounts;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Accounts.Api.Controllers.ListAccounts;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AccountController(
    IListAccountsUseCase listAccountsUseCase) : Controller
{
    private readonly IListAccountsUseCase _listAccountsUseCase = listAccountsUseCase;

    [HttpGet("All")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListAccountsOutput))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultFail[]))]
    public async Task<IActionResult> ListAsync([FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var input = new ListAccountsInput()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        var output = await _listAccountsUseCase
            .HandleAsync(input);

        return CreateResponse(output);
    }

    private IActionResult CreateResponse(Result<ListAccountsOutput> output)
    {
        if (output.Success)
            return Ok(output.GetContent());

        if (output.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(output.Failures);
        
        if (output.ContainsFailure("BALANCE_TEMPORARILY_UNAVAILABLE"))
            return StatusCode(503, output.Failures);

        return StatusCode(500, output.Failures);
    }
}