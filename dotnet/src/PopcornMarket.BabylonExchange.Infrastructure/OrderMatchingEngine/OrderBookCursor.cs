namespace PopcornMarket.BabylonExchange.Infrastructure.OrderMatchingEngine;
public record OrderBookCursor(decimal? LastPrice, DateTimeOffset? LastPlacedTimestamp);
