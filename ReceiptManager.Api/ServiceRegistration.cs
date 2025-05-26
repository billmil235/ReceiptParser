using ReceiptManager.Api.Services;

namespace ReceiptManager.Api;

public static class ServiceRegistration
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ReceiptReaderService>();
    }
}