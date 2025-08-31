namespace Bank.Transactions.Infrastructure.Gateways.KafkaBroker;

public interface ITopicCreators
{
    Task CreateTopicsAsync();
}