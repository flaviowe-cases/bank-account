using System.Text.Json;

namespace Bank.Transactions.Application.Serializers;

public class JsonSerializerDefault : IJsonSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public string Serialize<T>(T content)
        => JsonSerializer.Serialize(content, _jsonSerializerOptions);

    public T? Deserialize<T>(string json) 
        => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
}