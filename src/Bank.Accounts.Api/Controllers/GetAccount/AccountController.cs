using Asp.Versioning;
using Bank.Accounts.Application.UseCases.GetAccount;
using Bank.Commons.Applications.Factories.Results;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Accounts.Api.Controllers.GetAccount;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AccountController(
    IGetAccountUseCase getAccountUseCase) : Controller
{
    private readonly IGetAccountUseCase _getAccountUseCase = getAccountUseCase;

    [HttpGet("{account}")]
    [EndpointDescription("Returns account by account number")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAccountOutput))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultFail[]))]
    public async Task<IActionResult> GetAsync([FromRoute] string account)
    {
        var input = new GetAccountInput();
        
        if (Guid.TryParse(account, out var accountId))
            input.AccountId = accountId; 
        
        if (int.TryParse(account, out var accountNumber))
            input.AccountNumber = accountNumber; 
        
        var output = await _getAccountUseCase
            .HandleAsync(input);   
        
        return CreateResponse(output);  
    }

    private IActionResult CreateResponse(Result<GetAccountOutput> output)
    {
        if (output.Success)
            return Ok(output.GetContent());

        if (output.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(output.Failures);
        
        if (output.ContainsFailure("ACCOUNT_NOT_FOUND"))
            return NotFound(output.Failures);
        
        if (output.ContainsFailure("BALANCE_TEMPORARILY_UNAVAILABLE"))
            return StatusCode(503, output.Failures);    
        
        return StatusCode(500, output.Failures);
    }
}