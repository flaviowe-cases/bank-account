using System.Text.Json;

namespace Bank.Transactions.Application.Serializers;

public class JsonSerializerDefault : IJsonSerializer
{
    public string Serialize<T>(T content)
        => JsonSerializer.Serialize(content);

    public T? Deserialize<T>(string json) 
        => JsonSerializer.Deserialize<T>(json);
}