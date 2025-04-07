using FluentAssertions;
using NetArchTest.Rules;
using PopcornMarket.FinancialTimes.Application;
using PopcornMarket.SharedKernel.CQRS;

namespace PopcornMarket.FinancialTimes.ArchitectureTests.Application;

public class ApplicationTests
{
    [Fact]
    public void CommandHandlers_Should_BeSealed()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .BeSealed()
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes;
            var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
            result.IsSuccessful.Should().BeTrue($"the following CommandHandlers should be Sealed: {failingTypeNames}"); 
        }
    }
    
    [Fact]
    public void CommandHandlers_Should_BeInternal()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();
        
        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes;
            var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
            result.IsSuccessful.Should().BeTrue($"the following CommandHandlers should be Internal: {failingTypeNames}"); 
        }
    }
    
    [Fact]
    public void CommandHandlers_Should_HaveCommandHandlerPostfix()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();
        
        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes;
            var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
            result.IsSuccessful.Should().BeTrue($"the following CommandHandlers should have postfix \"CommandHandler\": {failingTypeNames}"); 
        }
    }
    
    [Fact]
    public void QueryHandlers_Should_BeSealed()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .BeSealed()
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes;
            var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
            result.IsSuccessful.Should().BeTrue($"the following QueryHandlers should be Sealed: {failingTypeNames}"); 
        }
    }
    
    [Fact]
    public void QueryHandlers_Should_BeInternal()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .NotBePublic()
            .GetResult();
        
        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes;
            var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
            result.IsSuccessful.Should().BeTrue($"the following QueryHandlers should be Internal: {failingTypeNames}"); 
        }
    }
    
    [Fact]
    public void QueryHandlers_Should_HaveQueryHandlerPostfix()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();
        
        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypes;
            var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
            result.IsSuccessful.Should().BeTrue($"the following QueryHandlers should have postfix \"QueryHandler\": {failingTypeNames}"); 
        }
    }
}
