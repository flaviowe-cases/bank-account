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

    public async Task<Result<BankAccount>> GetByAccountNumber(int accountNumber)
    {
        try
        {
            var requestUrl = $"api/v1/Account?accountNumber={accountNumber} ";

            using var response = await _httpClient.GetAsync(requestUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var account = _serializer.Deserialize<BankAccount>(responseContent);

                if (account == null)
                {
                    _logger.LogError("Failed to parse account: {ResponseContent}", responseContent);
                    
                    return _resultFactory.CreateFailure<BankAccount>(
                        "FAILED_PARSE_ACCOUNT",
                        "Failed to get account");
                }
                
                _resultFactory.CreateSuccess(account);  
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