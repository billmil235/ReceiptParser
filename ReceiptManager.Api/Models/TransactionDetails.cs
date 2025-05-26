namespace ReceiptManager.Api.Models;

public record TransactionDetails
{
    public required string CreditCardType { get; init; }
    public required string Last4 { get; init; }
    public required double Amount { get; init; }
    public required DateTime TransactionDateTime { get; init; }
}