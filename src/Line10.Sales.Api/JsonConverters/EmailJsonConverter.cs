using System.Text.Json;
using System.Text.Json.Serialization;
using Line10.Sales.Domain.ValueObjects;

namespace Line10.Sales.Api.JsonConverters;

public class EmailJsonConverter : JsonConverter<Email>
{
    public override Email Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var address = reader.GetString();
        return new Email(address);
    }

    public override void Write(Utf8JsonWriter writer, Email value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Address);
    }
}