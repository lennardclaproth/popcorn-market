namespace PopcornMarket.Messaging.Contracts.V1.Constants;

public static class TopicConstants
{
    private const string Version = "v1";

    public const string CompanyCreated = $"company.created.{Version}";
    public const string CompanyListed = $"company.listed.{Version}";

    public const string TradeExecuted = $"trade.executed.{Version}";

    public const string MarketDataPublished = $"marketdata.published.{Version}";
}
