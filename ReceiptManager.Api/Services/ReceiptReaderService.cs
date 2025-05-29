using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.AI;
using ReceiptManager.Api.Converters;
using ReceiptManager.Api.Models;

namespace ReceiptManager.Api.Services;

public class ReceiptReaderService
{
    private const string ReceiptPrompt = """
                                         Attached is an image of a cash register receipt.
                                         
                                         Extract the merchant name and address from the receipt.
                                         
                                         If available extract any line items from the receipt.  A line item consists of a description and a price.
                                         
                                         Extract the payment transaction information.  A receipt can have multiple payment types.
                                         
                                         A credit card, debit card or gift card transaction will have  the following information. 
                                         	- credit card type (visa, american express, mastercard, discover, or a gift card)
                                         	- the last 4 digits of the account number (Maybe donated similar to X#### or *####)
                                         	- transaction amount
                                         	- the date and time the transaction occurred.
                                         
                                         A cash transaction just has an amount.
                                         
                                         Each cash transaction will contain an amount and a date.
                                         
                                         Only provide a RFC8259 compliant JSON response following this format without deviation.
                                         
                                         [{
                                             "MerchantName": string,
                                             "Address": {
                                                "AddressLine1": string,
                                                "AddressLine2": string | null,
                                                "City": string,
                                                "State": string,
                                                "PostalCode": string
                                             } | null,
                                             "PhoneNumber": string | null,
                                             "LineItems": [ {
                                                "ItemDescription": string,
                                                "ItemPrice": double,
                                             }] | [],
                                             "TransactionDetails": [{
                                                "CreditCardType": string,
                                                "Last4": string,
                                                "Amount": double,
                                                "TransactionDateTime": datetime
                                             }],
                                             "TaxAmount": double | null,
                                             "ReceiptTotal": double | null,
                                         }]
                                         """;
    
    public async Task<Results<Ok<Receipt>, InternalServerError>> PersistUploadedFile(IFormFile file)
    {
	    var fileId = Guid.NewGuid();
        var receiptInformation = await ParseReceipt(file.OpenReadStream(), file.ContentType);
        return TypedResults.Ok(receiptInformation);
    }

    private static async Task<Receipt?> ParseReceipt(Stream file, string contentType)
    {
	    const string endpoint = "http://192.168.1.118:11434/";
	    const string modelName = "qwen2.5vl:7b";

	    using IChatClient client = new OllamaChatClient(endpoint, modelName);
	    using var ms = new MemoryStream();
	    await file.CopyToAsync(ms);

	    var message = new ChatMessage(ChatRole.System, ReceiptPrompt);
	    message.Contents.Add(new DataContent(ms.ToArray(), contentType));
	    
	    var result = await client.GetResponseAsync<Receipt>(message);
	    
	    return result.Result;
    }
}