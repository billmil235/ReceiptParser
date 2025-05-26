using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReceiptManager.Api.Converters;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        return dateString == null ? DateTime.MinValue : DateTime.Parse(dateString);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        //Don't implement this unless you're going to use the custom converter for serialization too
        throw new NotImplementedException();
    }
}