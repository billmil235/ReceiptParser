namespace ReceiptManager.Api.Models;

public record LineItem
{
    public required string ItemDescription { get; init; }
    public required double ItemPrice { get; init; }
}