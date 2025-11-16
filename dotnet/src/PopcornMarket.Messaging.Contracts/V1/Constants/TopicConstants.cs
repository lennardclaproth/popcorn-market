namespace PopcornMarket.Messaging.Contracts.V1.Constants;

public static class TopicConstants
{
    private const string Version = "v1";

    public const string CompanyCreated = $"company.created.{Version}";
    public const string CompanyListed = $"company.listed.{Version}";

    public const string OrderPartiallyFilled = $"order.partially_filled.{Version}";
    public const string OrderCancelled = $"order.cancelled.{Version}";
    public const string OrderFulfilled = $"order.fulfilled.{Version}";
    public const string OrderPartiallyCancelled = $"order.partially_cancelled.{Version}";
}
