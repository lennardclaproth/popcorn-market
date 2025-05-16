using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Popcorn.FinancialAtlas.Domain.Entities;
using PopcornMarket.SharedKernel.Primitives;

namespace PopcornMarket.FinancialAtlas.Persistence.Maps;

internal static class CompanyMap
{
    internal static void Configure()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(Company)))
        {
            // Create custom class map
            BsonClassMap.RegisterClassMap<Company>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

                cm.MapMember(c => c.Ticker)
                    .SetElementName("ticker");

                cm.MapMember(c => c.Name)
                    .SetElementName("name");

                cm.MapMember(c => c.Industry)
                    .SetElementName("industry");

                cm.MapMember(c => c.Description)
                    .SetElementName("description");

                cm.MapMember(c => c.Headquarters)
                    .SetElementName("headquarters");

                cm.MapMember(c => c.Ceo)
                    .SetElementName("ceo");

                cm.MapMember(c => c.FoundedYear)
                    .SetElementName("founded_year");

                cm.MapMember(c => c.Employees)
                    .SetElementName("employees");
                
                cm.MapMember(c => c.Region)
                    .SetElementName("region");
            });
        }
    }
}
