using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.AI;
using ReceiptManager.Api.Converters;
using ReceiptManager.Api.Models;

namespace ReceiptManager.Api.Services;

public class ReceiptReaderService
{
	private readonly JsonSerializerOptions _jSonConverterOptions;
	
	public ReceiptReaderService()
	{
		_jSonConverterOptions = new JsonSerializerOptions()
		{
			Converters = { new CustomDateTimeConverter() }
		};
	}
	
    private const string ReceiptPrompt = """
                                         Attached is an image of a cash register receipt.
                                         
                                         Typically, the merchant name and address will be at the top of the receipt.  Not all receipts will have an address.  If their is no merchant address, mark the line as NULL.
                                         If the receipt does have a merchant address, not all addresses will have a second address line so mark that as NULL if it does not exist.
                                         The receipt may contain a phone number.  If it does not, mark as null.  The phone number will typically be near the address or merchant name.
                                         
                                         Towards the middle of the receipt there may be line item information.  Each line will contain an item description, some blank space and a price.
                                         Not all receipts will have line items.  If the receipt does not contain line items initialize that section to an empty array.
                                         
                                         Payment transaction details will be found towards the bottom of the receipt.  Transaction types will be either cash or credit card.  
                                         Each credit card transaction will contain a credit card type, last 4 digits of the account number, the transaction amount and a transaction date with time.
                                         The credit card type will be one of the following: visa, american express, mastercard, discover, or gift card.
                                         The account number will be located bear the credit card type.  It may take the form XXXXXXXXXXXX1234 or *1234.

                                         Each cash transaction will contain an amount and a date.
                                         
                                         Extract this information into JSON formatted text.  The JSON should take the following form.
                                         
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

    private async Task<Receipt?> ParseReceipt(Stream file, string contentType)
    {
	    const string endpoint = "http://192.168.1.118:11434/";
	    const string modelName = "granite3.2-vision:2b-fp16";

	    using IChatClient client = new OllamaChatClient(endpoint, modelName);
	    using var ms = new MemoryStream();
	    await file.CopyToAsync(ms);

	    var message = new ChatMessage(ChatRole.User, ReceiptPrompt);
	    message.Contents.Add(new DataContent(ms.ToArray(), contentType));

	    var results = await client.GetResponseAsync(message);

	    using var responseStream = new MemoryStream(Encoding.UTF8.GetBytes(results.Text));
	    
	    return await JsonSerializer.DeserializeAsync<Receipt>(responseStream, _jSonConverterOptions);
    }
}