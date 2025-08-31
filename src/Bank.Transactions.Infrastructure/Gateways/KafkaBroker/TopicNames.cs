namespace Bank.Transactions.Infrastructure.Gateways.KafkaBroker;

public class TopicNames
{
    public const string ExecuteTransaction = "bank.transactions.execute.transaction";

    public readonly string[] AllTopics = [ExecuteTransaction];
}