using System.Net.Http.Json;
using Bank.Accounts.Application.Factories.Results;
using Bank.Accounts.Application.Gateways;
using Bank.Accounts.Application.Models;
using Bank.Accounts.Application.Serializers;
using Microsoft.Extensions.Logging;

namespace Bank.Accounts.Infrastructure.Gateways;

public class BankTransactionClient(
    ILogger<BankTransactionClient> logger,
    HttpClient httpClient,
    IJsonSerializer jsonSerializer,
    IResultFactory resultFactory) : IBankTransactionsClient
{
    private readonly ILogger<BankTransactionClient> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IJsonSerializer _jsonSerializer = jsonSerializer;
    private readonly IResultFactory _resultFactory = resultFactory;

    public async Task<Result<List<AmountApplication>>> GetAmountsAsync(List<Guid> accountIds)
    {
        try
        {
            var requestUrl = "api/v1/transactions/amounts";

            if (accountIds.Count != 0)
                requestUrl += "?" + string.Join("&", accountIds.Select(id => $"accountId={id}"));

            using var response = await _httpClient.GetAsync(requestUrl);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var amountsApplications = _jsonSerializer
                    .Deserialize<List<AmountApplication>>(content);

                amountsApplications ??= [];

                return _resultFactory
                    .CreateSuccess(amountsApplications);
            }
            else
            {
                _logger.LogError("Error getting amount applications {Ids} - {StatusCode} - {Content}",
                    accountIds,
                    response.StatusCode,
                    content);

                return _resultFactory.CreateFailure<List<AmountApplication>>(
                    "GET_AMOUNT_WITH_FAILURE",
                    $"Status Code: {response.StatusCode} - Response: {content}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get amount applications");
            return _resultFactory.CreateFailure<List<AmountApplication>>(
                "GET_AMOUNT_ERROR", e.Message);
        }
    }

    public async Task<Result<TransferApplication>> MakeTransferAsync(TransferApplication transfer)
    {
        try
        {
            var requestUrl = "api/v1/transactions";

            var requestContent = JsonContent.Create(transfer);

            using var response = await _httpClient.PostAsync(requestUrl, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return _resultFactory.CreateSuccess(transfer);

            _logger.LogCritical("Transfer was refused {AccountNumber} {StatusCode} - {Content}", 
                transfer.SourceAccountNumber, response.StatusCode, responseContent);
            
            return _resultFactory.CreateFailure<TransferApplication>(
                "MAKE_TRANSFER_WITH_FAILURE",
                $"Status Code: {response.StatusCode} - Response: {responseContent}");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to make transfer application");
            return _resultFactory.CreateFailure<TransferApplication>(
                "MAKE_TRANSFER_ERROR", e.Message);
        }
    }
}