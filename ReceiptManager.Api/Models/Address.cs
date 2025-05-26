namespace ReceiptManager.Api.Models;

public record Address
{
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? State { get; init; }
}