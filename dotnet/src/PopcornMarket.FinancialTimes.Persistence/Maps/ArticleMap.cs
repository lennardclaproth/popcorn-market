using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PopcornMarket.FinancialTimes.Domain.Entities;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialTimes.Persistence.Maps;

internal static class ArticleMap
{
    internal static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Article)))
        {
            BsonClassMap.RegisterClassMap<Article>(cm =>
            {
                cm.AutoMap(); // Automatically map properties
                cm.SetIgnoreExtraElements(true); // Ignore fields not in the model
                
                cm.MapMember(c => c.PublishDate)
                    .SetSerializer(new DateTimeSerializer(DateTimeKind.Utc))
                    .SetElementName("publish_date");

                cm.MapMember(c => c.Type)
                    .SetElementName("type");

                cm.MapMember(c => c.Headline)
                    .SetElementName("headline");

                cm.MapMember(c => c.Content)
                    .SetElementName("content");
            });
        }
    }
}
