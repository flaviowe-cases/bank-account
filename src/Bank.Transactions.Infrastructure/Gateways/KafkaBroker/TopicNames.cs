namespace Bank.Transactions.Infrastructure.Gateways.KafkaBroker;

public class TopicNames
{
    public const string ExecuteTransaction = "execute-transaction";

    public readonly string[] AllTopics = [ExecuteTransaction];
}