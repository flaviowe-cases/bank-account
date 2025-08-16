using Bank.Accounts.Application.Repositories;
using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Api;

public class AccountStubs(
    IAccountRepository accountRepository) : IAccountStubs
{
    private readonly IAccountRepository _accountRepository = accountRepository;

    public async Task AddAccountsAsync()
    {
        await _accountRepository.AddAsync(new Account
        {
            Id = Guid.Parse("9923bf1e-048f-469d-9b27-be35d08f1979"),
            Number = 10001,
            Name = "Dart Vader",
            Balance = 1500,
        });

        await _accountRepository.AddAsync(new Account
        {
            Id = Guid.Parse("2ff28f98-84e7-461e-9f38-c5df69c8624c"),
            Number = 10002,
            Name = "Yoda",
            Balance = 10000,
        });

        await _accountRepository.AddAsync(new Account
        {
            Id = Guid.Parse("c427d764-e8ed-4b45-9d58-40b7ad6aec4b"),
            Number = 10003,
            Name = "Obi-Wan",
            Balance = 5000,
        });

        await _accountRepository.AddAsync(new Account
        {
            Id = Guid.Parse("fb6b0990-c47d-4848-a94f-2471cce8bab0"),
            Number = 10004,
            Name = "Anakin Skywalker",
            Balance = 1000,
        });
    }
}