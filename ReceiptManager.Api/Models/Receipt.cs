namespace ReceiptManager.Api.Models;

public record Receipt
{
    public required string MerchantName { get; init; }
    public Address? Address { get; init; }
    public string? PhoneNumber { get; init; }
    public IEnumerable<LineItem>? LineItems { get; init; }
    public IEnumerable<TransactionDetails>? TransactionDetails { get; init; }
    public double? TaxAmount { get; init; }
    public double? ReceiptTotal { get; init; }
}