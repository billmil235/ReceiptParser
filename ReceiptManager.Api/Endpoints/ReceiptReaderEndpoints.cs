using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ReceiptManager.Api.Services;

namespace ReceiptManager.Api.Endpoints;

public static class ReceiptReaderEndpoints
{
    public static void RegisterReceiptReaderEndpoints(this WebApplication app)
    {
        app.MapPost("/Receipts/Upload", 
            async (IFormFile file, [FromServices] ReceiptReaderService receiptReaderService) => await receiptReaderService.PersistUploadedFile(file))
            .DisableAntiforgery();

        app.MapGet("/Receipts/Get",
            (Guid receiptId, [FromServices] ReceiptReaderService receiptReaderService) => Results.Ok());
    }
}