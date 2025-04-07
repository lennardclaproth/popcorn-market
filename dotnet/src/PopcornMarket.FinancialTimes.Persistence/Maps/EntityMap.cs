using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialTimes.Persistence.Maps;

internal static class EntityMap
{
    public static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Entity)))
        {
            BsonClassMap.RegisterClassMap<Entity>(cm =>
            {
                cm.MapMember(c => c.Id)
                    .SetSerializer(new GuidSerializer(BsonType.String))
                    .SetElementName("entity_id");
            });
        }
    }
}
