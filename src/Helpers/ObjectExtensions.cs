using MongoDB.Bson;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SensorWebApi.Helpers;

public static class ObjectExtensions
{
    public static string ToJsonString<TObject>(this TObject @object)
    {
        var output = "NULL";
        if (@object != null)
        {
            output = JsonSerializer.Serialize(@object, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        return $"[{@object?.GetType().Name}]:\r\n{output}";
    }

    public class BsonDocJsonConverter : JsonConverter<BsonDocument>
    {
        public override BsonDocument? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                string jsonStr = document.RootElement.GetRawText();
                BsonDocument.TryParse(jsonStr, out BsonDocument? bson);
                return bson;
            }
        }

        public override void Write(Utf8JsonWriter writer, BsonDocument value, JsonSerializerOptions options)
        {
            // Convert the BsonDocument to a JSON string
            string json = value.ToJson();

            // Write the JSON string as raw JSON
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                document.WriteTo(writer);
            }
        }
    }
}
