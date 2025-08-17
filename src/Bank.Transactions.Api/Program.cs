using Bank.Commons.Api;
using Bank.Commons.Api.Extensions;
using Bank.Transactions.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var bankAccountBaseAddress = Environment.GetEnvironmentVariable("BANK_ACCOUNT_BASE_ADDRESS");
var limitAmountTransferVariable = Environment.GetEnvironmentVariable("LIMIT_AMOUNT_TRANSFER");  

if (!decimal.TryParse(limitAmountTransferVariable, out var limitAmountTransfer))
    throw new ArgumentException(limitAmountTransferVariable);   

if (string.IsNullOrEmpty(bankAccountBaseAddress))
    throw new ArgumentNullException(nameof(bankAccountBaseAddress));    

var apiConfiguration = new ApiConfiguration()
{
    Title = "Bank Transactions API",
    Description = "Bank Transactions API provides a methods to handle transactions",
};

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCommonsApi(apiConfiguration);
builder.Services.AddBankTransactions(
    bankAccountBaseAddress,
    limitAmountTransfer);

var app = builder.Build();

app.UseCommonsApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();