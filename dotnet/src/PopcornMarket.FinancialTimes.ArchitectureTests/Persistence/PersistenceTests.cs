using FluentAssertions;
using NetArchTest.Rules;
using PopcornMarket.FinancialTimes.Persistence;
using PopcornMarket.FinancialTimes.Persistence.Extensions;

namespace PopcornMarket.FinancialTimes.ArchitectureTests.Persistence;

public class PersistenceTests
{
    [Fact]
    public void Infrastructure_Classes_Should_BeInternal()
    {
        var excludedClasses = new[] { 
            nameof(PersistenceAssemblyReference), 
            nameof(PersistenceExtensions)
        };
    
        var result = Types.InAssembly(PersistenceAssemblyReference.Assembly)
            .That()
            .AreClasses()
            .And()
            .DoNotHaveName(excludedClasses) // Exclude by name
            .Should()
            .NotBePublic()
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes;
            var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
            result.IsSuccessful.Should()
                .BeTrue($"the following classes should be marked as internal: {failingTypeNames}");
        }
    }
}
