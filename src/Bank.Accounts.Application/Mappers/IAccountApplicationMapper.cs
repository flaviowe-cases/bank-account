using Bank.Accounts.Application.Models;
using Bank.Accounts.Domain.Entities;

namespace Bank.Accounts.Application.Mappers;

public interface IAccountApplicationMapper
{
    List<AccountApplication> ToApplication(List<Transaction> accounts);
    AccountApplication ToApplication(Transaction transaction);
}