using System.Net;
using Bank.Transactions.Application.Factories.Results;
using Bank.Transactions.Application.Gateways;
using Bank.Transactions.Application.Models;
using Bank.Transactions.Application.Serializers;
using Microsoft.Extensions.Logging;

namespace Bank.Transactions.Infrastructure.Gateways;

public class BankAccountClient(
    ILogger<BankAccountClient> logger,
    IJsonSerializer serializer,
    HttpClient httpClient,
    IResultFactory resultFactory) : IBankAccountClient
{
    private readonly ILogger<BankAccountClient> _logger = logger;
    private readonly IJsonSerializer _serializer = serializer;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IResultFactory _resultFactory = resultFactory;

    public Task<Result<BankAccount>> GetByAccountAsync(int accountNumber)
        => GetAccountAsync(accountNumber.ToString());
    
    public Task<Result<BankAccount>> GetByAccountAsync(Guid accountId)
        => GetAccountAsync(accountId.ToString());
    
    private async Task<Result<BankAccount>> GetAccountAsync(string account)
    {
        try
        {
            var requestUrl = $"api/v1/Account/{account}";

            using var response = await _httpClient.GetAsync(requestUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var accountResponse = _serializer.Deserialize<BankAccountResponse>(responseContent);

                if (accountResponse == null)
                {
                    _logger.LogError("Failed to parse account: {ResponseContent}", responseContent);

                    return _resultFactory.CreateFailure<BankAccount>(
                        "FAILED_PARSE_ACCOUNT",
                        "Failed to get account");
                }

                var accountBank = accountResponse.Account;
                return _resultFactory.CreateSuccess(accountBank);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return _resultFactory.CreateFailure<BankAccount>(
                    "ACCOUNT_NOT_FOUND",
                    "Account not found");
            }

            _logger.LogError("Failed to get account: {ResponseContent}", responseContent);

            return _resultFactory.CreateFailure<BankAccount>(
                "FAILED_TO_GET_ACCOUNT",
                "Failed to get account");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to make transfer application");
            return _resultFactory.CreateFailure<BankAccount>(
                "GET_ACCOUNT_ERROR", e.Message);
        }
    }
}