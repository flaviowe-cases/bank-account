using Bank.Transactions.Application.Repositories;
using Bank.Transactions.Domain.Entities;
using MongoDB.Driver;

namespace Bank.Transactions.Infrastructure.Repositories;

public class TransactionRepository(IMongoDatabase mongoDatabase) : ITransactionRepository
{
    private readonly IMongoCollection<Transaction> _collection = mongoDatabase
        .GetCollection<Transaction>("transactions");

    public async Task AddAsync(Transaction transaction)
    {
        await _collection
            .InsertOneAsync(transaction);
    }

    public async Task<List<Transaction>> GetByAccountIdAsync(
        Guid accountId,
        TransactionStatusType? status = null)
    {
        var filters = new List<FilterDefinition<Transaction>>
        {
            Builders<Transaction>.Filter.Or(
                Builders<Transaction>.Filter.Eq(
                    transaction => transaction.SourceAccountId, accountId),
                Builders<Transaction>.Filter.Eq(
                    transaction => transaction.DestinationAccountId, accountId))
        };

        if (status != null)
            filters.Add(
                Builders<Transaction>.Filter.Eq(
                    transaction => transaction.Status, status));

        var query = Builders<Transaction>.Filter.And(filters);

        var transactions = await _collection
            .FindAsync(query);
        return transactions.ToList();
    }
}