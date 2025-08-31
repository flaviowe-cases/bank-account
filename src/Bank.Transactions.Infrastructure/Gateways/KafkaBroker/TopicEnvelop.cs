namespace Bank.Transactions.Infrastructure.Gateways.KafkaBroker;

public class TopicEnvelop<T>
{
    public Guid Id { get; set; }
    public required T Message { get; init; }
}