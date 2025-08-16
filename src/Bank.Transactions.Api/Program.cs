using Asp.Versioning.ApiExplorer;
using Bank.Transactions.Api;
using Bank.Transactions.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var bankAccountBaseAddress = Environment.GetEnvironmentVariable("BANK_ACCOUNT_BASE_ADDRESS");
var limitAmountTransferVariable = Environment.GetEnvironmentVariable("LIMIT_AMOUNT_TRANSFER");  

if (!decimal.TryParse(limitAmountTransferVariable, out var limitAmountTransfer))
    throw new ArgumentException(limitAmountTransferVariable);   

if (string.IsNullOrEmpty(bankAccountBaseAddress))
    throw new ArgumentNullException(nameof(bankAccountBaseAddress));    

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddBankTransactions(
    bankAccountBaseAddress,
    limitAmountTransfer);

builder.Services
    .AddApiVersioning(options => { options.ReportApiVersions = true; })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToLowerInvariant());
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();