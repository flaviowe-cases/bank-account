using Asp.Versioning.ApiExplorer;
using Bank.Accounts.Api;
using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var bankTransactionBaseAddress = Environment.GetEnvironmentVariable("BANK_TRANSACTION_BASE_ADDRESS");

if (string.IsNullOrEmpty(bankTransactionBaseAddress))
    throw new ArgumentNullException(nameof(bankTransactionBaseAddress));

builder
    .Services
    .AddSwaggerGen()
    .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
    .AddBankAccounts(bankTransactionBaseAddress)
    .AddControllers();

builder.Services.AddScoped<IAccountStubs, AccountStubs>();

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
                description.GroupName.ToUpperInvariant());
    });
}

app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<IAccountStubs>()
        .AddAccountsAsync();

app.Run();