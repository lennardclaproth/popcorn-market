using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PopcornMarket.FinancialTimes.Domain.Enums;

namespace PopcornMarket.FinancialTimes.Persistence.Maps;

internal static class ArticleTypeMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(ArticleType)))
        {
            // Registering the enum serializer to store as string in MongoDB
            BsonSerializer.RegisterSerializer(typeof(ArticleType), new EnumSerializer<ArticleType>(BsonType.String));
        }
    }
}
