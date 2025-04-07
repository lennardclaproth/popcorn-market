using FluentAssertions;
using NetArchTest.Rules;
using PopcornMarket.FinancialTimes.Api;
using PopcornMarket.FinancialTimes.Application;
using PopcornMarket.FinancialTimes.Domain;
using PopcornMarket.FinancialTimes.Persistence;

namespace PopcornMarket.FinancialTimes.ArchitectureTests;

public class LayerTests
{
    [Fact]
    public void Domain_Should_NotHaveDependencyOnApplication()
    {
        var result = Types.InAssembly(DomainAssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssemblyReference.Assembly.ToString())
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Domain_Should_NotHaveDependencyOnPersistence()
    {
        var result = Types.InAssembly(DomainAssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(PersistenceAssemblyReference.Assembly.ToString())
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Domain_Should_NotHaveDependencyOnPresentation()
    {
        var result = Types.InAssembly(DomainAssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssemblyReference.Assembly.ToString())
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_NotHaveDependencyOnInfrastructure()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(PersistenceAssemblyReference.Assembly.ToString())
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Application_Should_NotHaveDependencyOnPresentation()
    {
        var result = Types.InAssembly(ApplicationAssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssemblyReference.Assembly.ToString())
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_NotHaveDependencyOnPresentation()
    {
        var result = Types.InAssembly(PersistenceAssemblyReference.Assembly)
            .Should()
            .NotHaveDependencyOn(PresentationAssemblyReference.Assembly.ToString())
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
