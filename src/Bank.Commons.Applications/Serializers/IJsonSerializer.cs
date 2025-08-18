namespace Bank.Commons.Applications.Serializers;

public interface IJsonSerializer
{
    string Serialize<T>(T content);
    T? Deserialize<T>(string json);
}