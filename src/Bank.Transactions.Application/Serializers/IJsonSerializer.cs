namespace Bank.Transactions.Application.Serializers;

public interface IJsonSerializer
{
    string Serialize<T>(T content);
    T? Deserialize<T>(string json);
}