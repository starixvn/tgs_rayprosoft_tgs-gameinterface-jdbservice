using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.GameInterface.JDBService.Core.JsonConverters;

internal class JDBDateTimeConverter : JsonConverter<DateTimeOffset>
{
    private readonly TimeZoneInfo _timeZone;

    public JDBDateTimeConverter()
        => _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var unspecifyDate = DateTime.ParseExact(reader.GetString()!, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

        return new DateTimeOffset(unspecifyDate, _timeZone.GetUtcOffset(unspecifyDate));
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) => throw new NotImplementedException();
}
