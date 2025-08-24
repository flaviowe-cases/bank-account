using Bank.Transactions.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Bank.Transactions.Infrastructure.Extensions;

public static class MongoClientExtensions
{
    public static IMongoClient MappingEntities(this IMongoClient mongoClient)
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Transaction)))
        {
            BsonClassMap.RegisterClassMap<Transaction>(mapper =>
            {
                mapper.AutoMap();
                mapper.MapIdMember(x => x.Id)
                    .SetElementName("id")
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                mapper.MapMember(transaction => transaction.SourceAccountId)
                    .SetElementName("sourceAccountId")
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                mapper.MapMember(transaction => transaction.DestinationAccountId)
                    .SetElementName("destinationAccountId")
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                mapper.MapMember(transaction => transaction.Amount)
                    .SetElementName("amount");
                mapper.MapMember(transaction => transaction.Status)
                    .SetElementName("status");
                mapper.MapMember(transaction => transaction.Comments)
                    .SetElementName("comments");
                mapper.MapMember(transaction => transaction.TimeStamp)
                    .SetElementName("timestamp");
            });
        }
        return mongoClient;
    }
}