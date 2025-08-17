using Asp.Versioning;
using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.UseCases.AddAccount;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Accounts.Api.Controllers.AddAccount;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AccountController(
    IAddAccountUseCase addAccountUseCase) : Controller
{
    private readonly IAddAccountUseCase _addAccountUseCase = addAccountUseCase;

    [HttpPost]
    [EndpointDescription("Adds a new account")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ResultFail[]))]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(ResultFail[]))]
    public async Task<IActionResult> PostAsync([FromBody] AddAccountRequest request)
    {
        var input = CreateInput(request);
        var output = await _addAccountUseCase.HandleAsync(input);
        return CreateResponse(output);
    }

    private static AddAccountInput CreateInput(AddAccountRequest request)
        => new AddAccountInput
        {
            AccountId = request.AccountId ?? Guid.Empty,
            Name = request.Name ?? "",
            AccountNumber = request.AccountNumber,
            Amount = request.InitialAmount,
        };

    private IActionResult CreateResponse(Result<AddAccountOutput> output)
    {
        if (output.Success)
        {
            var content = output.GetContent();
            var uri = $"/api/v1/Account/{content.AccountId}";
            return Created(uri, content);
        }

        if (output.ContainsFailure("INVALID_FIELDS"))
            return BadRequest(output.Failures);

        if (output.ContainsFailure("ACCOUNT_ID_ALREADY_EXISTS"))
            return Conflict(output.Failures);

        if (output.ContainsFailure("ACCOUNT_NUMBER_ALREADY_EXISTS"))
            return Conflict(output.Failures);

        if (output.ContainsFailure("DEPOSIT_TEMPORARILY_UNAVAILABLE"))
            return StatusCode(503, output.Failures);

        return StatusCode(500, output.Failures);
    }
}